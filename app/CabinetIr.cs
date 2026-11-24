using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ArchetypeGojiraX
{
    // Respostas de impulso (IR) da caixa: as internas são geradas por código a partir do
    // tipo de caixa e dos microfones; o usuário também pode carregar qualquer IR em .wav.
    internal static class CabinetIrLibrary
    {
        public const int SampleRate = GuitarAudio.SampleRate;
        public const int Length = Convolver.MaxLength;

        private static readonly object sync = new object();
        private static volatile float[] custom;
        private static string customName, cachedKey;
        private static float[] cachedIr;

        static CabinetIrLibrary()
        {
            try
            {
                string saved = SettingsPath;
                if (File.Exists(saved))
                {
                    string path = File.ReadAllText(saved, Encoding.UTF8).Trim();
                    if (File.Exists(path)) SetCustom(path);
                }
            }
            catch (Exception) { custom = null; customName = null; }
        }

        public static float[] Custom { get { return custom; } }
        public static string CustomName { get { return customName; } }

        public static void LoadCustom(string path)
        {
            SetCustom(path);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                File.WriteAllText(SettingsPath, path, Encoding.UTF8);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        public static void ClearCustom()
        {
            custom = null;
            customName = null;
            try { if (File.Exists(SettingsPath)) File.Delete(SettingsPath); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private static void SetCustom(string path)
        {
            int rate;
            float[] samples = WavFile.ReadFirstChannel(path, out rate);
            float[] ir = Resample(samples, rate, SampleRate, Length);
            FadeOut(ir, Math.Min(256, ir.Length / 4));
            Normalize(ir);
            custom = ir;
            customName = Path.GetFileName(path);
        }

        private static string SettingsPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "appguitarra", "ir-personalizada.txt");
            }
        }

        public static float[] ForPreset(PresetData preset)
        {
            float[] user = custom;
            if (user != null) return user;
            int cab = Math.Max(0, Math.Min(2, preset.CabType));
            string key = string.Format(CultureInfo.InvariantCulture,
                "{0}|{1}{2}|{3:F3}|{4:F3}|{5:F1}|{6}|{7}{8}|{9:F3}|{10:F3}|{11:F1}|{12}",
                cab, preset.Mic1Active, preset.Mic1Type, preset.Mic1Pos, preset.Mic1Dist, preset.Mic1LevelDb,
                preset.Mic1PhaseInvert, preset.Mic2Active, preset.Mic2Type, preset.Mic2Pos, preset.Mic2Dist,
                preset.Mic2LevelDb, preset.Mic2PhaseInvert);
            lock (sync)
            {
                if (key == cachedKey) return cachedIr;
                var ir = new float[Length];
                if (preset.Mic1Active)
                    AddMic(ir, cab, preset.Mic1Type, preset.Mic1Pos, preset.Mic1Dist, preset.Mic1LevelDb, preset.Mic1PhaseInvert);
                if (preset.Mic2Active)
                    AddMic(ir, cab, preset.Mic2Type, preset.Mic2Pos, preset.Mic2Dist, preset.Mic2LevelDb, preset.Mic2PhaseInvert);
                FadeOut(ir, 512);
                cachedKey = key;
                cachedIr = ir;
                return ir;
            }
        }

        // ---- Síntese de uma caixa microfonada ----

        private static void AddMic(float[] target, int cab, int micType, float position, float distance, float levelDb, bool invert)
        {
            var response = BuildMic(cab, micType, position, distance);
            Normalize(response);
            float gain = GuitarSettings.FromDb(levelDb) * (invert ? -1 : 1);
            for (int i = 0; i < target.Length; i++) target[i] += response[i] * gain;
        }

        private static float[] BuildMic(int cab, int micType, float position, float distance)
        {
            double rate = SampleRate;
            var x = new double[Length];

            // Som direto (mais longe = chega depois, o que cria o filtro em pente ao misturar 2 microfones)
            int direct = (int)Math.Round(distance * 0.0015 * rate);
            x[direct] = 1;

            // Reflexões dentro do gabinete; caixa aberta (CLN) tem a onda de trás bem marcada
            double[,] reflections = cab == 0
                ? new double[,] { { 0.45, -0.18 }, { 1.1, 0.12 }, { 2.6, -0.22 }, { 4.0, 0.08 } }
                : new double[,] { { 0.35, 0.20 }, { 0.9, -0.15 }, { 1.8, 0.10 }, { 3.3, 0.06 } };
            double roomGain = 0.6 + 0.8 * distance;
            for (int i = 0; i < reflections.GetLength(0); i++)
            {
                int at = direct + (int)Math.Round(reflections[i, 0] * 0.001 * rate);
                if (at < Length) x[at] += reflections[i, 1] * roomGain;
            }

            // Cauda curta do corpo da caixa (ruído com decaimento exponencial, sempre a mesma semente)
            var random = new Random(1234 + cab * 31 + micType);
            double decay = (cab == 0 ? 0.004 : 0.006) * rate;
            for (int i = direct; i < Length; i++)
                x[i] += (random.NextDouble() * 2 - 1) * 0.02 * Math.Exp(-(i - direct) / decay);

            // Alto-falante: ressonância grave, picos de presença e queda dos agudos (quebra do cone)
            double resonance, resonanceQ, cutoff;
            var chain = new System.Collections.Generic.List<BiquadCoefficients>();
            if (cab == 0)
            {
                resonance = 75; resonanceQ = 0.8; cutoff = 7000;
                chain.Add(Filters.Peak(rate, 200, 1.0, -2));
                chain.Add(Filters.Peak(rate, 2500, 1.2, 3));
            }
            else if (cab == 1)
            {
                resonance = 95; resonanceQ = 1.4; cutoff = 5000;
                chain.Add(Filters.Peak(rate, 400, 1.0, 1));
                chain.Add(Filters.Peak(rate, 1600, 1.0, 4));
            }
            else
            {
                resonance = 100; resonanceQ = 1.6; cutoff = 5500;
                chain.Add(Filters.Peak(rate, 600, 1.0, -2));
                chain.Add(Filters.Peak(rate, 2400, 1.4, 3.5));
                chain.Add(Filters.Peak(rate, 4500, 3.0, 2));
            }
            chain.Add(Filters.HighPass(rate, resonance, resonanceQ));

            // Posição do microfone: no centro do cone é mais brilhante, na borda é mais escuro.
            // Distância: perto tem mais grave (efeito de proximidade) e menos ambiente.
            cutoff *= (1.35 - 0.75 * position) * (1 - 0.25 * distance);
            chain.Add(Filters.HighShelf(rate, 3500, 3 - 8 * position));
            chain.Add(Filters.LowShelf(rate, 160, 4 * (1 - distance) - 1));
            chain.Add(Filters.Peak(rate, cutoff * 0.7, 3.0, 2));
            chain.Add(Filters.LowPass(rate, cutoff, 0.54));
            chain.Add(Filters.LowPass(rate, cutoff, 1.31));
            chain.Add(Filters.LowPass(rate, 12000, 0.707));

            AddMicColor(chain, rate, micType);

            var filter = new Biquad();
            foreach (var coefficients in chain)
            {
                filter.Reset();
                filter.Set(coefficients);
                for (int i = 0; i < Length; i++) x[i] = filter.Process(x[i]);
            }
            var result = new float[Length];
            for (int i = 0; i < Length; i++) result[i] = (float)x[i];
            return result;
        }

        // Cor de cada microfone (na ordem da lista do app)
        private static void AddMicColor(System.Collections.Generic.List<BiquadCoefficients> chain, double rate, int micType)
        {
            switch (micType)
            {
                case 0: // Dynamic 57
                    chain.Add(Filters.HighPass(rate, 60, 0.707));
                    chain.Add(Filters.Peak(rate, 5000, 1.2, 4));
                    break;
                case 1: // Dynamic 57 fora do eixo
                    chain.Add(Filters.HighPass(rate, 60, 0.707));
                    chain.Add(Filters.Peak(rate, 4000, 1.2, 2));
                    chain.Add(Filters.HighShelf(rate, 3000, -5));
                    break;
                case 2: // Dynamic 421
                    chain.Add(Filters.Peak(rate, 4500, 1.0, 2.5));
                    chain.Add(Filters.LowShelf(rate, 150, 1.5));
                    break;
                case 3: // Condenser 184
                    chain.Add(Filters.HighShelf(rate, 6000, 3));
                    chain.Add(Filters.LowShelf(rate, 150, -2));
                    break;
                case 4: // Condenser 414
                    chain.Add(Filters.HighShelf(rate, 8000, 2));
                    chain.Add(Filters.Peak(rate, 3000, 1.0, 1.5));
                    break;
                case 5: // Ribbon 121
                    chain.Add(Filters.HighShelf(rate, 3500, -6));
                    chain.Add(Filters.LowShelf(rate, 200, 3));
                    break;
                default: // Ribbon 160
                    chain.Add(Filters.HighShelf(rate, 4000, -4));
                    chain.Add(Filters.Peak(rate, 1000, 1.0, 1.5));
                    chain.Add(Filters.LowShelf(rate, 150, 2));
                    break;
            }
        }

        // ---- Utilidades ----

        // Ajusta o volume para que a média de energia entre 80 Hz e 5 kHz fique em 0 dB.
        private static void Normalize(float[] ir)
        {
            const int points = 40;
            double total = 0;
            for (int p = 0; p < points; p++)
            {
                double frequency = 80 * Math.Pow(5000.0 / 80, p / (double)(points - 1));
                double w = 2 * Math.PI * frequency / SampleRate, re = 0, im = 0;
                double stepCos = Math.Cos(w), stepSin = Math.Sin(w), phaseCos = 1, phaseSin = 0;
                for (int i = 0; i < ir.Length; i++)
                {
                    re += ir[i] * phaseCos;
                    im -= ir[i] * phaseSin;
                    double rotated = phaseCos * stepCos - phaseSin * stepSin;
                    phaseSin = phaseSin * stepCos + phaseCos * stepSin;
                    phaseCos = rotated;
                }
                total += re * re + im * im;
            }
            double scale = total > 0 ? 1 / Math.Sqrt(total / points) : 0;
            if (scale == 0) throw new InvalidDataException("A IR está vazia ou em silêncio.");
            for (int i = 0; i < ir.Length; i++) ir[i] = (float)(ir[i] * scale);
        }

        private static void FadeOut(float[] ir, int length)
        {
            for (int i = 0; i < length; i++)
                ir[ir.Length - 1 - i] *= (float)(0.5 - 0.5 * Math.Cos(Math.PI * i / length));
        }

        // Converte a taxa de amostragem com interpolação sinc janelada e corta no tamanho máximo.
        private static float[] Resample(float[] input, int fromRate, int toRate, int maxLength)
        {
            if (fromRate == toRate)
            {
                var copy = new float[Math.Min(input.Length, maxLength)];
                Array.Copy(input, copy, copy.Length);
                return copy;
            }
            double ratio = fromRate / (double)toRate, cut = Math.Min(1.0, 1 / ratio);
            const int taps = 16;
            int length = (int)Math.Min(maxLength, Math.Ceiling(input.Length / ratio));
            var output = new float[length];
            for (int n = 0; n < length; n++)
            {
                double t = n * ratio, sum = 0;
                int center = (int)Math.Floor(t);
                for (int k = center - taps; k <= center + taps; k++)
                {
                    if (k < 0 || k >= input.Length) continue;
                    double d = t - k, arg = Math.PI * d * cut;
                    double sinc = Math.Abs(arg) < 1e-9 ? 1 : Math.Sin(arg) / arg;
                    double window = 0.5 + 0.5 * Math.Cos(Math.PI * d / (taps + 1));
                    sum += input[k] * sinc * cut * window;
                }
                output[n] = (float)sum;
            }
            return output;
        }
    }

    // Convolução direta (sem atraso extra), com transição suave quando a IR muda.
    internal sealed class Convolver
    {
        public const int MaxLength = 2048;
        private const int FadeLength = 960;
        private readonly float[] history = new float[MaxLength * 2];
        private float[] current, previous;
        private int position, fade;

        public void SetImpulse(float[] impulse)
        {
            if (impulse == current) return;
            previous = current;
            current = impulse;
            fade = previous == null || current == null ? 0 : FadeLength;
        }

        public float Process(float sample, bool enabled)
        {
            position = position == 0 ? MaxLength - 1 : position - 1;
            history[position] = sample;
            history[position + MaxLength] = sample;
            if (!enabled || current == null) return sample;
            float output = Dot(current);
            if (fade > 0)
            {
                float t = fade / (float)FadeLength;
                output = output * (1 - t) + Dot(previous) * t;
                if (--fade == 0) previous = null;
            }
            return output;
        }

        private float Dot(float[] impulse)
        {
            float[] h = history;
            int start = position, n = Math.Min(impulse.Length, MaxLength);
            float a = 0, b = 0, c = 0, d = 0;
            int k = 0;
            for (; k + 3 < n; k += 4)
            {
                a += impulse[k] * h[start + k];
                b += impulse[k + 1] * h[start + k + 1];
                c += impulse[k + 2] * h[start + k + 2];
                d += impulse[k + 3] * h[start + k + 3];
            }
            for (; k < n; k++) a += impulse[k] * h[start + k];
            return a + b + c + d;
        }
    }

    // Leitor de .wav: PCM 16/24/32 bits e ponto flutuante 32/64 bits; usa só o primeiro canal.
    internal static class WavFile
    {
        public static float[] ReadFirstChannel(string path, out int sampleRate)
        {
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                if (Tag(reader) != "RIFF") throw Invalid();
                reader.ReadInt32();
                if (Tag(reader) != "WAVE") throw Invalid();
                int format = 0, channels = 0, bits = 0;
                sampleRate = 0;
                while (reader.BaseStream.Position + 8 <= reader.BaseStream.Length)
                {
                    string id = Tag(reader);
                    long size = reader.ReadUInt32();
                    long next = reader.BaseStream.Position + size + (size & 1);
                    if (id == "fmt ")
                    {
                        format = reader.ReadUInt16();
                        channels = reader.ReadUInt16();
                        sampleRate = reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadUInt16();
                        bits = reader.ReadUInt16();
                        if (format == 0xFFFE && size >= 26)
                        {
                            reader.ReadUInt16();
                            reader.ReadUInt16();
                            reader.ReadUInt32();
                            format = reader.ReadUInt16();
                        }
                    }
                    else if (id == "data")
                    {
                        if (channels <= 0 || sampleRate <= 0) throw Invalid();
                        size = Math.Min(size, reader.BaseStream.Length - reader.BaseStream.Position);
                        return Decode(reader.ReadBytes((int)size), format, channels, bits);
                    }
                    if (next > reader.BaseStream.Length) break;
                    reader.BaseStream.Position = next;
                }
                throw Invalid();
            }
        }

        private static float[] Decode(byte[] data, int format, int channels, int bits)
        {
            int bytes = bits / 8, frame = bytes * channels;
            if (frame == 0) throw Invalid();
            int frames = data.Length / frame;
            var output = new float[frames];
            for (int i = 0; i < frames; i++)
            {
                int at = i * frame;
                if (format == 1 && bits == 16) output[i] = BitConverter.ToInt16(data, at) / 32768f;
                else if (format == 1 && bits == 24) output[i] = ((data[at] << 8 | data[at + 1] << 16 | data[at + 2] << 24) >> 8) / 8388608f;
                else if (format == 1 && bits == 32) output[i] = BitConverter.ToInt32(data, at) / 2147483648f;
                else if (format == 3 && bits == 32) output[i] = BitConverter.ToSingle(data, at);
                else if (format == 3 && bits == 64) output[i] = (float)BitConverter.ToDouble(data, at);
                else throw new InvalidDataException("Formato de .wav não suportado (" + bits + " bits, tipo " + format + ").");
            }
            return output;
        }

        private static string Tag(BinaryReader reader)
        {
            return Encoding.ASCII.GetString(reader.ReadBytes(4));
        }

        private static Exception Invalid()
        {
            return new InvalidDataException("O arquivo não é um .wav válido.");
        }
    }
}
