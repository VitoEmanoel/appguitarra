using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ArchetypeGojiraX
{
    internal sealed class GuitarAudio : IDisposable
    {
        public const int SampleRate = 48000;
        private readonly ManualResetEvent stop = new ManualResetEvent(false);
        private readonly object meterLock = new object();
        private readonly float[] waveform = new float[128];
        private float inputPeak, outputPeak;
        private bool gateOpen;
        private volatile GuitarSettings parameters;
        private volatile bool running;
        private volatile bool starting;
        private volatile string error;
        private Thread worker;

        public bool Running { get { return running; } }
        public bool Starting { get { return starting; } }
        public string Error { get { return error; } }

        public void SetParameters(GuitarSettings settings) { parameters = settings; }

        public void Start(AudioEndpoint input, AudioEndpoint output, int channel, GuitarSettings settings)
        {
            Stop();
            parameters = settings;
            error = null;
            lock (meterLock)
            {
                Array.Clear(waveform, 0, waveform.Length);
                inputPeak = outputPeak = 0;
                gateOpen = false;
            }
            stop.Reset();
            starting = true;
            worker = new Thread(() => Run(input, output, channel));
            worker.Name = "Guitar audio";
            worker.IsBackground = true;
            worker.Priority = ThreadPriority.AboveNormal;
            worker.SetApartmentState(ApartmentState.STA);
            worker.Start();
        }

        public void Stop()
        {
            if (worker == null) return;
            stop.Set();
            worker.Join();
            worker = null;
        }

        public void CopyMeters(float[] destination, out float input, out float output, out bool open)
        {
            lock (meterLock)
            {
                input = inputPeak;
                output = outputPeak;
                open = gateOpen;
                Array.Copy(waveform, destination, waveform.Length);
            }
        }

        private void Run(AudioEndpoint input, AudioEndpoint output, int channel)
        {
            using (var captured = new AutoResetEvent(false))
            using (var rendered = new AutoResetEvent(false))
            {
                WindowsAudio.IAudioClient inputClient = null, outputClient = null;
                WindowsAudio.ICaptureClient capture = null;
                WindowsAudio.IRenderClient render = null;
                string action = "abrir a entrada " + input.Name;
                try
                {
                    inputClient = WindowsAudio.Open(input.Id, SampleRate, captured.SafeWaitHandle.DangerousGetHandle());
                    capture = WindowsAudio.GetService<WindowsAudio.ICaptureClient>(inputClient);
                    action = "abrir a saída " + output.Name;
                    outputClient = WindowsAudio.Open(output.Id, SampleRate, rendered.SafeWaitHandle.DangerousGetHandle());
                    render = WindowsAudio.GetService<WindowsAudio.IRenderClient>(outputClient);
                    uint inputFrames, outputFrames;
                    inputClient.GetBufferSize(out inputFrames);
                    outputClient.GetBufferSize(out outputFrames);
                    var inputSamples = new short[checked((int)inputFrames * 2)];
                    var outputSamples = new short[checked((int)outputFrames * 2)];
                    var samples = new SampleQueue(Math.Max((int)inputFrames, (int)outputFrames) * 2);
                    var processor = new GuitarProcessor(SampleRate);
                    var preview = new float[waveform.Length];

                    action = "iniciar a monitoração";
                    IntPtr unused;
                    render.GetBuffer(outputFrames, out unused);
                    render.ReleaseBuffer(outputFrames, WindowsAudio.Silent);
                    inputClient.Start();
                    outputClient.Start();
                    running = true;
                    starting = false;

                    action = "manter a monitoração";
                    var signals = new WaitHandle[] { stop, captured, rendered };
                    while (WaitHandle.WaitAny(signals, 100) != 0)
                    {
                        Capture(capture, inputSamples, channel, processor, samples, preview);
                        uint padding;
                        outputClient.GetCurrentPadding(out padding);
                        Write(render, outputSamples, samples, outputFrames - padding);
                    }
                }
                catch (Exception ex)
                {
                    error = "Não foi possível " + action + " (0x" + ex.HResult.ToString("X8") + ").";
                }
                finally
                {
                    StopClient(inputClient);
                    StopClient(outputClient);
                    WindowsAudio.Release(capture);
                    WindowsAudio.Release(render);
                    WindowsAudio.Release(inputClient);
                    WindowsAudio.Release(outputClient);
                    running = false;
                    starting = false;
                }
            }
        }

        private void Capture(WindowsAudio.ICaptureClient client, short[] buffer, int channel,
            GuitarProcessor processor, SampleQueue queue, float[] preview)
        {
            uint available;
            client.GetNextPacketSize(out available);
            while (available != 0)
            {
                IntPtr data;
                uint frames, flags;
                client.GetBuffer(out data, out frames, out flags, IntPtr.Zero, IntPtr.Zero);
                if (frames == 0) break;
                bool silent = (flags & WindowsAudio.Silent) != 0;
                try
                {
                    if (!silent) Marshal.Copy(data, buffer, 0, (int)frames * 2);
                }
                finally { client.ReleaseBuffer(frames); }

                processor.SetParameters(parameters);
                float input = 0, output = 0;
                int nextPreview = 0;
                for (int i = 0; i < frames; i++)
                {
                    float gained;
                    float sample = silent ? 0 : buffer[i * 2 + channel] / 32768f;
                    float processed = processor.Process(sample, out gained);
                    input = Math.Max(input, Math.Abs(gained));
                    output = Math.Max(output, Math.Abs(processed));
                    queue.Add((short)(processed * short.MaxValue));
                    while (nextPreview < preview.Length && nextPreview * frames / preview.Length == i)
                        preview[nextPreview++] = processed;
                }
                lock (meterLock)
                {
                    inputPeak = Math.Max(input, inputPeak * 0.9f);
                    outputPeak = Math.Max(output, outputPeak * 0.9f);
                    gateOpen = processor.GateOpen;
                    Array.Copy(preview, waveform, waveform.Length);
                }
                client.GetNextPacketSize(out available);
            }
        }

        private static void Write(WindowsAudio.IRenderClient client, short[] buffer, SampleQueue samples, uint frames)
        {
            if (frames == 0) return;
            IntPtr data;
            client.GetBuffer(frames, out data);
            uint flags = WindowsAudio.Silent;
            try
            {
                if (samples.Count == 0) return;
                for (int i = 0; i < frames; i++)
                {
                    short sample = samples.Take();
                    buffer[i * 2] = buffer[i * 2 + 1] = sample;
                }
                Marshal.Copy(buffer, 0, data, (int)frames * 2);
                flags = 0;
            }
            finally { client.ReleaseBuffer(frames, flags); }
        }

        private static void StopClient(WindowsAudio.IAudioClient client)
        {
            if (client != null) client.Stop();
        }

        public void Dispose()
        {
            Stop();
            stop.Dispose();
        }

        private sealed class SampleQueue
        {
            private readonly short[] samples;
            private int read, write;
            public int Count { get; private set; }

            public SampleQueue(int capacity) { samples = new short[capacity]; }

            public void Add(short sample)
            {
                if (Count == samples.Length) Take();
                samples[write] = sample;
                write = (write + 1) % samples.Length;
                Count++;
            }

            public short Take()
            {
                if (Count == 0) return 0;
                short sample = samples[read];
                read = (read + 1) % samples.Length;
                Count--;
                return sample;
            }
        }
    }
}
