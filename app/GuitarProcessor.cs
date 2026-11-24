using System;

namespace ArchetypeGojiraX
{
    internal sealed class GuitarSettings
    {
        public const int MaxStages = 3;

        public readonly float InputGain, OutputGain, GateThreshold;
        public readonly bool Muted, Overdrive, Distortion, Amp, Cab;

        // Pedais
        public readonly float OverdriveDrive, OverdriveLevel, OverdriveTone;
        public readonly float DistortionDrive, DistortionLevel, DistortionTone, DistortionBandwidth;

        // Pré-amplificador: estágios de válvula em sequência
        public readonly int Stages;
        public readonly float[] StageGain = new float[MaxStages], StageBias = new float[MaxStages];
        public readonly float[] StageTightFrequency = new float[MaxStages], StageTightDb = new float[MaxStages];
        public readonly float[] StageLowPass = new float[MaxStages];
        public readonly float[] StageEqFrequency = new float[MaxStages], StageEqDb = new float[MaxStages];

        // Equalização do amplificador e estágio de potência
        public readonly float BassDb, MidDb, TrebleDb, MidFrequency, PresenceDb, DepthDb;
        public readonly float PowerDrive, AmpLevel;

        // Caixa (resposta de impulso a 48 kHz)
        public readonly float[] CabinetIr;

        public GuitarSettings(PresetData preset, bool muted)
        {
            int amp = Math.Max(0, Math.Min(2, preset.AmpType));
            InputGain = FromDb(preset.InputGainDb);
            OutputGain = FromDb(preset.OutputGainDb) * 0.35f;
            GateThreshold = preset.GateActive ? FromDb(preset.GateThresholdDb) : 0;
            Muted = muted;

            Overdrive = preset.PreFxSectionActive && preset.OdActive;
            OverdriveDrive = 1.5f + 50f * preset.OdDist * preset.OdDist;
            OverdriveLevel = FromDb((preset.OdLevel - 0.5f) * 24);
            OverdriveTone = 900 + preset.OdTone * 5200;

            Distortion = preset.PreFxSectionActive && preset.DrtActive;
            DistortionDrive = 2f + 400f * preset.DrtDist * preset.DrtDist;
            DistortionBandwidth = 3000 + (1 - preset.DrtDist) * 9000;
            DistortionLevel = FromDb((preset.DrtVol - 0.5f) * 24) * 0.5f;
            DistortionTone = 700 + (1 - preset.DrtFilter) * 4300;

            Amp = preset.AmpSectionActive && preset.AmpPower[amp];
            float gain = preset.AmpGain[amp];
            if (amp == 0)
            {
                // CLN: um estágio, limpo até o ganho alto, com "bright" mais forte em ganho baixo
                Stages = 1;
                SetStage(0, 0.8f + 6f * gain * gain, 0.08f, 100, 0, 14000, 3000, preset.ClnBright ? 6f * (1 - gain) : 0);
                MidFrequency = 450;
            }
            else if (amp == 1)
            {
                // RST: dois estágios, crunch com médios à frente
                Stages = 2;
                SetStage(0, 1.5f + 10f * gain, 0.20f, 120, -6, 12000, 700, 2);
                SetStage(1, 1.5f + 6f * gain, -0.15f, 150, -4, 8000, 1200, 3);
                MidFrequency = 700;
            }
            else
            {
                // HOT: três estágios, graves firmes antes da saturação e médios empurrados
                Stages = 3;
                SetStage(0, 3f + 14f * gain, 0.25f, 150, -10, 10000, 850, 5);
                SetStage(1, 2f + 8f * gain, -0.20f, 200, -6, 7500, 1800, 2);
                SetStage(2, 1.5f + 3f * gain, 0.15f, 100, -3, 5500, 500, -2);
                MidFrequency = 650;
            }
            BassDb = (preset.AmpBass[amp] - 0.5f) * 20;
            MidDb = (preset.AmpMid[amp] - 0.5f) * 20;
            TrebleDb = (preset.AmpTreble[amp] - 0.5f) * 20;
            PresenceDb = (preset.AmpPresence[amp] - 0.5f) * 12;
            DepthDb = (preset.AmpDepth[amp] - 0.5f) * 12;
            PowerDrive = 0.5f + 1.5f * preset.AmpMaster[amp];
            AmpLevel = FromDb((preset.AmpLevel[amp] - 0.5f) * 24) * AmpMakeup[amp];

            Cab = preset.CabSectionActive && (preset.Mic1Active || preset.Mic2Active || CabinetIrLibrary.Custom != null);
            CabinetIr = Cab ? CabinetIrLibrary.ForPreset(preset) : null;
        }

        // Compensa a diferença de volume entre os três amplificadores (calibrado com o teste offline).
        private static readonly float[] AmpMakeup = { 1.0f, 1.0f, 1.0f };

        // tight: corte suave de graves antes de saturar (como o capacitor de catodo de uma válvula)
        private void SetStage(int index, float gain, float bias, float tightFrequency, float tightDb,
            float lowPass, float eqFrequency, float eqDb)
        {
            StageGain[index] = gain;
            StageBias[index] = bias;
            StageTightFrequency[index] = tightFrequency;
            StageTightDb[index] = tightDb;
            StageLowPass[index] = lowPass;
            StageEqFrequency[index] = eqFrequency;
            StageEqDb[index] = eqDb;
        }

        internal static float FromDb(float db) { return (float)Math.Pow(10, db / 20); }
    }

    internal sealed class GuitarProcessor
    {
        private const int Factor = Oversampler.Factor;
        private readonly double rate, fastRate;
        private readonly Oversampler oversampler;
        private readonly double[] block = new double[Factor];
        private readonly float envelopeRelease, gateSpeed;
        private readonly double sagRelease;

        private readonly Biquad inputHighPass = new Biquad();
        private readonly Biquad odHighPass = new Biquad(), odTone = new Biquad();
        private readonly Biquad drtHighPass = new Biquad(), drtBandwidth = new Biquad(), drtTone = new Biquad();
        private readonly Biquad[] stageCoupling = NewFilters(GuitarSettings.MaxStages);
        private readonly Biquad[] stageTight = NewFilters(GuitarSettings.MaxStages);
        private readonly Biquad[] stageEq = NewFilters(GuitarSettings.MaxStages);
        private readonly Biquad[] stageLowPass = NewFilters(GuitarSettings.MaxStages);
        private readonly Biquad bass = new Biquad(), mid = new Biquad(), treble = new Biquad();
        private readonly Biquad presence = new Biquad(), depth = new Biquad(), transformer = new Biquad();
        private readonly Convolver cabinet = new Convolver();

        private GuitarSettings settings;
        private float envelope, gate, outputGain;
        private double sag;

        public GuitarProcessor(int sampleRate)
        {
            rate = sampleRate;
            fastRate = sampleRate * (double)Factor;
            oversampler = new Oversampler(sampleRate);
            envelopeRelease = (float)Math.Exp(-1.0 / (0.055 * sampleRate));
            gateSpeed = 1 - (float)Math.Exp(-1.0 / (0.003 * sampleRate));
            sagRelease = Math.Exp(-1.0 / (0.08 * fastRate));
            inputHighPass.Set(Filters.HighPass(rate, 20, 0.707));
            odHighPass.Set(Filters.HighPass(fastRate, 720, 0.5));
            drtHighPass.Set(Filters.HighPass(fastRate, 70, 0.707));
            transformer.Set(Filters.LowPass(fastRate, 11000, 0.707));
            foreach (var coupling in stageCoupling) coupling.Set(Filters.HighPass(fastRate, 25, 0.5));
        }

        public void SetParameters(GuitarSettings next)
        {
            if (settings == next) return;
            settings = next;
            odTone.Set(Filters.LowPass(fastRate, next.OverdriveTone, 0.707));
            drtBandwidth.Set(Filters.LowPass(fastRate, next.DistortionBandwidth, 0.707));
            drtTone.Set(Filters.LowPass(fastRate, next.DistortionTone, 0.707));
            for (int i = 0; i < next.Stages; i++)
            {
                stageTight[i].Set(Filters.LowShelf(fastRate, next.StageTightFrequency[i], next.StageTightDb[i]));
                stageEq[i].Set(Filters.Peak(fastRate, next.StageEqFrequency[i], 0.8, next.StageEqDb[i]));
                stageLowPass[i].Set(Filters.LowPass(fastRate, next.StageLowPass[i], 0.707));
            }
            bass.Set(Filters.LowShelf(fastRate, 110, next.BassDb));
            mid.Set(Filters.Peak(fastRate, next.MidFrequency, 0.8, next.MidDb));
            treble.Set(Filters.HighShelf(fastRate, 2200, next.TrebleDb));
            presence.Set(Filters.HighShelf(fastRate, 3800, next.PresenceDb));
            depth.Set(Filters.Peak(fastRate, 95, 1.1, next.DepthDb));
            cabinet.SetImpulse(next.CabinetIr);
        }

        public float Process(float sample, out float input)
        {
            input = sample * settings.InputGain;
            double signal = inputHighPass.Process(input);
            envelope = Math.Max(Math.Abs((float)signal), envelope * envelopeRelease);
            gate += ((GateOpen ? 1f : 0f) - gate) * gateSpeed;
            signal = signal * gate + 1e-18;

            oversampler.Up(signal, block);
            for (int k = 0; k < Factor; k++) block[k] = ProcessFast(block[k]);
            float output = (float)oversampler.Down(block);

            if (Math.Abs(output) < 1e-15f) output = 0;
            output = cabinet.Process(output, settings.Cab);

            float target = settings.Muted ? 0 : settings.OutputGain;
            outputGain += (target - outputGain) * gateSpeed;
            return (float)Math.Tanh(output * outputGain);
        }

        public bool GateOpen { get { return envelope >= settings.GateThreshold; } }

        // Tudo que distorce roda a 4x a taxa de amostragem.
        private double ProcessFast(double x)
        {
            GuitarSettings s = settings;
            if (s.Overdrive)
            {
                // Overdrive estilo "tube screamer": o sinal original passa inteiro e só os médios
                // e agudos ganham distorção por cima, então o grave continua presente.
                double clipped = Math.Tanh(odHighPass.Process(x) * s.OverdriveDrive);
                x = odTone.Process(x + clipped * 0.5) * s.OverdriveLevel;
            }
            if (s.Distortion)
            {
                double driven = drtBandwidth.Process(drtHighPass.Process(x) * s.DistortionDrive);
                x = drtTone.Process(Math.Tanh(driven)) * s.DistortionLevel;
            }
            if (!s.Amp) return x;

            for (int i = 0; i < s.Stages; i++)
            {
                x = stageEq[i].Process(stageTight[i].Process(stageCoupling[i].Process(x)));
                x = Tube(x * s.StageGain[i], s.StageBias[i]);
                x = stageLowPass[i].Process(x);
            }
            x = treble.Process(mid.Process(bass.Process(x)));

            // Estágio de potência: compressão ("sag") quando é empurrado, presença e "depth".
            double driven2 = x * s.PowerDrive / (1 + 0.35 * sag);
            double power = Tube(driven2, 0.1);
            sag = Math.Max(Math.Abs(power), sag * sagRelease);
            x = transformer.Process(depth.Process(presence.Process(power)));
            return x * s.AmpLevel;
        }

        // Saturação assimétrica: o "bias" desloca a curva e gera harmônicos pares, como numa válvula.
        private static double Tube(double x, double bias)
        {
            return Math.Tanh(x + bias) - Math.Tanh(bias);
        }

        private static Biquad[] NewFilters(int count)
        {
            var filters = new Biquad[count];
            for (int i = 0; i < count; i++) filters[i] = new Biquad();
            return filters;
        }
    }
}
