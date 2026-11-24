using System;

namespace ArchetypeGojiraX
{
    internal struct BiquadCoefficients
    {
        public double B0, B1, B2, A1, A2;
    }

    // Filtro biquad (Direct Form II transposta) em precisão dupla.
    internal sealed class Biquad
    {
        private double b0 = 1, b1, b2, a1, a2, z1, z2;

        public void Set(BiquadCoefficients c)
        {
            b0 = c.B0; b1 = c.B1; b2 = c.B2; a1 = c.A1; a2 = c.A2;
        }

        public double Process(double x)
        {
            double y = b0 * x + z1;
            z1 = b1 * x - a1 * y + z2;
            z2 = b2 * x - a2 * y;
            return y;
        }

        public void Reset() { z1 = z2 = 0; }
    }

    // Fórmulas do "Audio EQ Cookbook" (Robert Bristow-Johnson).
    internal static class Filters
    {
        public static BiquadCoefficients Identity
        {
            get { return new BiquadCoefficients { B0 = 1 }; }
        }

        public static BiquadCoefficients LowPass(double rate, double frequency, double q)
        {
            double cos, alpha;
            Prepare(rate, frequency, q, out cos, out alpha);
            return Normalize((1 - cos) / 2, 1 - cos, (1 - cos) / 2, 1 + alpha, -2 * cos, 1 - alpha);
        }

        public static BiquadCoefficients HighPass(double rate, double frequency, double q)
        {
            double cos, alpha;
            Prepare(rate, frequency, q, out cos, out alpha);
            return Normalize((1 + cos) / 2, -(1 + cos), (1 + cos) / 2, 1 + alpha, -2 * cos, 1 - alpha);
        }

        public static BiquadCoefficients Peak(double rate, double frequency, double q, double gainDb)
        {
            if (Math.Abs(gainDb) < 0.01) return Identity;
            double cos, alpha;
            Prepare(rate, frequency, q, out cos, out alpha);
            double a = Math.Pow(10, gainDb / 40);
            return Normalize(1 + alpha * a, -2 * cos, 1 - alpha * a, 1 + alpha / a, -2 * cos, 1 - alpha / a);
        }

        public static BiquadCoefficients LowShelf(double rate, double frequency, double gainDb)
        {
            if (Math.Abs(gainDb) < 0.01) return Identity;
            double cos, alpha;
            Prepare(rate, frequency, Math.Sqrt(0.5), out cos, out alpha);
            double a = Math.Pow(10, gainDb / 40), root = 2 * Math.Sqrt(a) * alpha;
            return Normalize(
                a * ((a + 1) - (a - 1) * cos + root),
                2 * a * ((a - 1) - (a + 1) * cos),
                a * ((a + 1) - (a - 1) * cos - root),
                (a + 1) + (a - 1) * cos + root,
                -2 * ((a - 1) + (a + 1) * cos),
                (a + 1) + (a - 1) * cos - root);
        }

        public static BiquadCoefficients HighShelf(double rate, double frequency, double gainDb)
        {
            if (Math.Abs(gainDb) < 0.01) return Identity;
            double cos, alpha;
            Prepare(rate, frequency, Math.Sqrt(0.5), out cos, out alpha);
            double a = Math.Pow(10, gainDb / 40), root = 2 * Math.Sqrt(a) * alpha;
            return Normalize(
                a * ((a + 1) + (a - 1) * cos + root),
                -2 * a * ((a - 1) + (a + 1) * cos),
                a * ((a + 1) + (a - 1) * cos - root),
                (a + 1) - (a - 1) * cos + root,
                2 * ((a - 1) - (a + 1) * cos),
                (a + 1) - (a - 1) * cos - root);
        }

        private static void Prepare(double rate, double frequency, double q, out double cos, out double alpha)
        {
            double w = 2 * Math.PI * Math.Min(frequency, rate * 0.49) / rate;
            cos = Math.Cos(w);
            alpha = Math.Sin(w) / (2 * q);
        }

        private static BiquadCoefficients Normalize(double b0, double b1, double b2, double a0, double a1, double a2)
        {
            return new BiquadCoefficients { B0 = b0 / a0, B1 = b1 / a0, B2 = b2 / a0, A1 = a1 / a0, A2 = a2 / a0 };
        }
    }

    // Sobreamostragem 4x com filtros Butterworth de 8ª ordem na subida e na descida.
    // Processar a distorção a 192 kHz evita o "aliasing" (aspereza digital) dos harmônicos agudos.
    internal sealed class Oversampler
    {
        public const int Factor = 4;
        private static readonly double[] ButterworthQ = { 0.50979558, 0.60134489, 0.89997622, 2.56291545 };
        private readonly Biquad[] up = new Biquad[ButterworthQ.Length];
        private readonly Biquad[] down = new Biquad[ButterworthQ.Length];

        public Oversampler(double baseRate)
        {
            double rate = baseRate * Factor, cutoff = baseRate * 0.45;
            for (int i = 0; i < ButterworthQ.Length; i++)
            {
                up[i] = new Biquad();
                down[i] = new Biquad();
                up[i].Set(Filters.LowPass(rate, cutoff, ButterworthQ[i]));
                down[i].Set(Filters.LowPass(rate, cutoff, ButterworthQ[i]));
            }
        }

        public void Up(double sample, double[] output)
        {
            for (int k = 0; k < Factor; k++)
            {
                double value = k == 0 ? sample * Factor : 0;
                for (int i = 0; i < up.Length; i++) value = up[i].Process(value);
                output[k] = value;
            }
        }

        public double Down(double[] input)
        {
            double result = 0;
            for (int k = 0; k < Factor; k++)
            {
                double value = input[k];
                for (int i = 0; i < down.Length; i++) value = down[i].Process(value);
                if (k == 0) result = value;
            }
            return result;
        }
    }
}
