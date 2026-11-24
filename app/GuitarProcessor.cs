using System;

namespace ArchetypeGojiraX
{
    internal sealed class GuitarSettings
    {
        public readonly float InputGain, OutputGain, GateThreshold;
        public readonly bool Muted, Overdrive, Distortion, Amp, Cab;
        public readonly float OverdriveDrive, OverdriveLevel, OverdriveTone;
        public readonly float DistortionDrive, DistortionLevel, DistortionTone;
        public readonly float AmpDrive, AmpLevel, Bass, Mid, Treble, CabCutoff;

        public GuitarSettings(PresetData preset, bool muted)
        {
            int amp = preset.AmpType;
            int cab = preset.CabType;
            InputGain = FromDb(preset.InputGainDb);
            OutputGain = FromDb(preset.OutputGainDb) * 0.35f;
            GateThreshold = preset.GateActive ? FromDb(preset.GateThresholdDb) : 0;
            Muted = muted;
            Overdrive = preset.PreFxSectionActive && preset.OdActive;
            Distortion = preset.PreFxSectionActive && preset.DrtActive;
            OverdriveDrive = 1.2f + preset.OdDist * 8f;
            OverdriveLevel = preset.OdLevel * 0.8f;
            OverdriveTone = 900 + preset.OdTone * 5200;
            DistortionDrive = 2f + preset.DrtDist * 22f;
            DistortionLevel = preset.DrtVol * 0.7f;
            DistortionTone = 700 + (1 - preset.DrtFilter) * 4300;
            Amp = preset.AmpSectionActive && preset.AmpPower[amp];
            AmpDrive = amp == 0 ? 1.3f + 5 * preset.AmpGain[amp]
                : amp == 1 ? 4 + 22 * preset.AmpGain[amp] : 7 + 40 * preset.AmpGain[amp];
            AmpLevel = preset.AmpMaster[amp] * FromDb((preset.AmpLevel[amp] - 0.5f) * 24);
            Bass = FromDb((preset.AmpBass[amp] - 0.5f) * 14 + (preset.AmpDepth[amp] - 0.5f) * 6);
            Mid = FromDb((preset.AmpMid[amp] - 0.5f) * 14);
            Treble = FromDb((preset.AmpTreble[amp] - 0.5f) * 14 + (preset.AmpPresence[amp] - 0.5f) * 6
                + (amp == 0 && preset.ClnBright ? 2 : 0));
            Cab = preset.CabSectionActive && (preset.Mic1Active || preset.Mic2Active);
            CabCutoff = cab == 0 ? 5600 : cab == 1 ? 4500 : 3800;
        }

        private static float FromDb(float db) { return (float)Math.Pow(10, db / 20); }
    }

    internal sealed class GuitarProcessor
    {
        private readonly int sampleRate;
        private readonly float envelopeRelease, gateSpeed, dcCut, bassCut, trebleCut, cabLowCut;
        private GuitarSettings settings;
        private float envelope, gate, outputGain;
        private float dc, overdriveLow, distortionLow, bass, mid, cabBass, cabTreble, cabSmooth;
        private float odCut, drtCut, cabHighCut;

        public GuitarProcessor(int sampleRate)
        {
            this.sampleRate = sampleRate;
            envelopeRelease = (float)Math.Exp(-1.0 / (0.055 * sampleRate));
            gateSpeed = 1 - (float)Math.Exp(-1.0 / (0.003 * sampleRate));
            dcCut = Coefficient(28);
            bassCut = Coefficient(240);
            trebleCut = Coefficient(1750);
            cabLowCut = Coefficient(90);
        }

        public void SetParameters(GuitarSettings next)
        {
            if (settings == next) return;
            settings = next;
            odCut = Coefficient(next.OverdriveTone);
            drtCut = Coefficient(next.DistortionTone);
            cabHighCut = Coefficient(next.CabCutoff);
        }

        public float Process(float sample, out float input)
        {
            input = sample * settings.InputGain;
            float signal = input - LowPass(input, dcCut, ref dc);
            envelope = Math.Max(Math.Abs(signal), envelope * envelopeRelease);
            gate += ((GateOpen ? 1f : 0f) - gate) * gateSpeed;
            signal *= gate;

            float od = LowPass(Clip(signal * settings.OverdriveDrive), odCut, ref overdriveLow);
            if (settings.Overdrive) signal = od * settings.OverdriveLevel;
            float distortion = LowPass(Clip(signal * settings.DistortionDrive), drtCut, ref distortionLow);
            if (settings.Distortion) signal = distortion * settings.DistortionLevel;

            float shaped = Clip(signal * settings.AmpDrive);
            float lows = LowPass(shaped, bassCut, ref bass);
            float lowAndMid = LowPass(shaped, trebleCut, ref mid);
            if (settings.Amp)
            {
                signal = (lows * settings.Bass + (lowAndMid - lows) * settings.Mid
                    + (shaped - lowAndMid) * settings.Treble) * settings.AmpLevel;
            }

            float cabSignal = signal - LowPass(signal, cabLowCut, ref cabBass);
            cabSignal = LowPass(cabSignal, cabHighCut, ref cabTreble);
            cabSignal = LowPass(cabSignal, cabHighCut, ref cabSmooth);
            if (settings.Cab) signal = cabSignal;

            float target = settings.Muted ? 0 : settings.OutputGain;
            outputGain += (target - outputGain) * gateSpeed;
            return Math.Max(-0.98f, Math.Min(0.98f, signal * outputGain));
        }

        public bool GateOpen { get { return envelope >= settings.GateThreshold; } }

        private float Coefficient(float frequency)
        {
            return 1 - (float)Math.Exp(-2 * Math.PI * frequency / sampleRate);
        }

        private static float LowPass(float input, float coefficient, ref float memory)
        {
            memory += coefficient * (input - memory);
            return memory;
        }

        private static float Clip(float signal) { return (float)Math.Tanh(signal); }
    }
}
