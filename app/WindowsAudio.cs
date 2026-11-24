using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArchetypeGojiraX
{
    internal sealed class AudioEndpoint
    {
        public readonly string Id;
        public readonly string Name;

        public AudioEndpoint(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() { return Name; }
    }

    internal static class WindowsAudio
    {
        internal const int Render = 0, Capture = 1;
        internal const uint Silent = 2;
        private const uint Active = 1, ReadOnly = 0, ClsctxAll = 23;
        private const uint EventCallback = 0x00040000, AutoConvertPcm = 0x80000000, DefaultConversionQuality = 0x08000000;
        private static readonly Guid EnumeratorClass = new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E");

        internal static AudioEndpoint[] GetEndpoints(int flow)
        {
            var endpoints = new List<AudioEndpoint>();
            var enumerator = CreateEnumerator();
            IDeviceCollection collection = null;
            try
            {
                enumerator.EnumAudioEndpoints(flow, Active, out collection);
                uint count;
                collection.GetCount(out count);
                for (uint index = 0; index < count; index++)
                {
                    IDevice device;
                    collection.Item(index, out device);
                    try
                    {
                        string id;
                        device.GetId(out id);
                        endpoints.Add(new AudioEndpoint(id, ReadName(device, index)));
                    }
                    finally { Release(device); }
                }
                return endpoints.ToArray();
            }
            finally
            {
                Release(collection);
                Release(enumerator);
            }
        }

        private static string ReadName(IDevice device, uint index)
        {
            IPropertyStore store = null;
            var key = new PropertyKey { Format = new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"), Id = 14 };
            var value = new PropertyValue();
            try
            {
                device.OpenPropertyStore(ReadOnly, out store);
                store.GetValue(ref key, out value);
                if (value.Type == 31)
                    return Marshal.PtrToStringUni(value.Pointer);
            }
            catch (COMException) { }
            finally
            {
                PropVariantClear(ref value);
                Release(store);
            }
            return "Dispositivo " + (index + 1);
        }

        internal static IAudioClient Open(string id, int sampleRate, IntPtr signal)
        {
            IDeviceEnumerator enumerator = CreateEnumerator();
            IDevice device = null;
            IAudioClient client = null;
            try
            {
                enumerator.GetDevice(id, out device);
                var iid = typeof(IAudioClient).GUID;
                object activated;
                device.Activate(ref iid, ClsctxAll, IntPtr.Zero, out activated);
                client = (IAudioClient)activated;
                var format = new WaveFormat {
                    Tag = 1, Channels = 2, SamplesPerSecond = sampleRate,
                    BytesPerSecond = sampleRate * 4, BlockAlign = 4, BitsPerSample = 16
                };
                client.Initialize(0, EventCallback | AutoConvertPcm | DefaultConversionQuality, 0, 0, ref format, IntPtr.Zero);
                client.SetEventHandle(signal);
                return client;
            }
            catch
            {
                Release(client);
                throw;
            }
            finally
            {
                Release(device);
                Release(enumerator);
            }
        }

        internal static T GetService<T>(IAudioClient client)
        {
            var iid = typeof(T).GUID;
            object service;
            client.GetService(ref iid, out service);
            return (T)service;
        }

        private static IDeviceEnumerator CreateEnumerator()
        {
            return (IDeviceEnumerator)Activator.CreateInstance(Type.GetTypeFromCLSID(EnumeratorClass, true));
        }

        internal static void Release(object instance)
        {
            if (instance != null) Marshal.ReleaseComObject(instance);
        }

        [DllImport("ole32.dll")]
        private static extern int PropVariantClear(ref PropertyValue value);

        [StructLayout(LayoutKind.Sequential)]
        private struct PropertyKey { public Guid Format; public uint Id; }

        [StructLayout(LayoutKind.Sequential)]
        private struct CountedPointers { public uint Count; public IntPtr Pointer; }

        [StructLayout(LayoutKind.Explicit)]
        private struct PropertyValue
        {
            [FieldOffset(0)] public ushort Type;
            [FieldOffset(8)] public IntPtr Pointer;
            [FieldOffset(8)] private CountedPointers array;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct WaveFormat
        {
            public ushort Tag, Channels;
            public int SamplesPerSecond, BytesPerSecond;
            public ushort BlockAlign, BitsPerSample, ExtensionSize;
        }

        [ComImport, Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDeviceEnumerator
        {
            void EnumAudioEndpoints(int flow, uint stateMask, out IDeviceCollection devices);
            void GetDefaultAudioEndpoint(int flow, int role, out IDevice device);
            void GetDevice([MarshalAs(UnmanagedType.LPWStr)] string id, out IDevice device);
        }

        [ComImport, Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDeviceCollection
        {
            void GetCount(out uint count);
            void Item(uint index, out IDevice device);
        }

        [ComImport, Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDevice
        {
            void Activate(ref Guid iid, uint context, IntPtr options, [MarshalAs(UnmanagedType.IUnknown)] out object instance);
            void OpenPropertyStore(uint access, out IPropertyStore store);
            void GetId([MarshalAs(UnmanagedType.LPWStr)] out string id);
        }

        [ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPropertyStore
        {
            void GetCount(out uint count);
            void GetAt(uint index, out PropertyKey key);
            void GetValue(ref PropertyKey key, out PropertyValue value);
        }

        [ComImport, Guid("1CB9AD4C-DBFA-4C32-B178-C2F568A703B2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioClient
        {
            void Initialize(int mode, uint flags, long duration, long periodicity, ref WaveFormat format, IntPtr session);
            void GetBufferSize(out uint frames);
            void GetStreamLatency(out long latency);
            void GetCurrentPadding(out uint frames);
            void IsFormatSupported(int mode, IntPtr format, IntPtr closest);
            void GetMixFormat(out IntPtr format);
            void GetDevicePeriod(out long defaultPeriod, out long minimumPeriod);
            void Start();
            [PreserveSig] int Stop();
            void Reset();
            void SetEventHandle(IntPtr signal);
            void GetService(ref Guid iid, [MarshalAs(UnmanagedType.IUnknown)] out object service);
        }

        [ComImport, Guid("C8ADBD64-E71E-48A0-A4DE-185C395CD317"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface ICaptureClient
        {
            void GetBuffer(out IntPtr data, out uint frames, out uint flags, IntPtr devicePosition, IntPtr timestamp);
            void ReleaseBuffer(uint frames);
            void GetNextPacketSize(out uint frames);
        }

        [ComImport, Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IRenderClient
        {
            void GetBuffer(uint frames, out IntPtr data);
            void ReleaseBuffer(uint frames, uint flags);
        }
    }
}
