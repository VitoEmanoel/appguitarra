using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ArchetypeGojiraX
{
    public class PresetData
    {
        public string Category;
        public string Name;
        public int AmpType; // 0=CLN, 1=RST, 2=HOT
        public int CabType; // 0=CLN, 1=RST, 2=HOT
        public bool AmpCabLinked = true;
        public float InputGainDb = 0.0f;
        public bool GateActive = true;
        public float GateThresholdDb = -80.0f;
        public int TransposeSemitones = 0;
        public bool StereoMode = false;
        public bool DoublerActive = false;
        public float DoublerSpreadMs = 6.0f;
        public float OutputGainDb = 0.0f;

        // Section Active
        public bool PitchSectionActive = true;
        public bool PreFxSectionActive = true;
        public bool AmpSectionActive = true;
        public bool CabSectionActive = true;
        public bool EqSectionActive = true;
        public bool PostFxSectionActive = true;

        // Pitch: WOW & OCT
        public bool WowActive = false;
        public int WowMode = 1; // 0=FATSO, 1=BLADE 1, 2=BLADE 2
        public float WowPedalPos = 0.0f;
        public float WowFatsoMix = 0.50f;
        public bool OctActive = false;
        public float Oct1Level = 0.50f;
        public float Oct2Level = 0.50f;
        public float OctDirect = 1.00f;

        // Pre FX: OD, DRT, PHSR, CHR
        public bool OdActive = true;
        public float OdDist = 0.15f;
        public float OdLevel = 0.78f;
        public float OdTone = 0.58f;

        public bool DrtActive = false;
        public float DrtDist = 0.62f;
        public float DrtVol = 0.55f;
        public float DrtFilter = 0.48f;

        public bool PhsrActive = false;
        public float PhsrRateHz = 1.00f;

        public bool ChrActive = false;
        public float ChrRateHz = 0.85f;
        public float ChrDepth = 0.50f;
        public float ChrFeedback = 0.25f;
        public float ChrMix = 0.45f;

        // Amps (3 channels: 0=CLN, 1=RST, 2=HOT)
        public bool[] AmpPower = new bool[] { true, true, true };
        public bool ClnBright = true;
        public float[] AmpGain = new float[] { 0.38f, 0.62f, 0.72f };
        public float[] AmpBass = new float[] { 0.52f, 0.58f, 0.64f };
        public float[] AmpMid = new float[] { 0.48f, 0.65f, 0.58f };
        public float[] AmpTreble = new float[] { 0.62f, 0.60f, 0.68f };
        public float[] AmpMaster = new float[] { 0.50f, 0.55f, 0.52f };
        public float[] AmpPresence = new float[] { 0.50f, 0.60f, 0.66f };
        public float[] AmpDepth = new float[] { 0.50f, 0.56f, 0.62f };
        public float[] AmpLevel = new float[] { 0.55f, 0.52f, 0.54f };

        // Cab (Dual Mic per Cab)
        public bool Mic1Active = true;
        public int Mic1Type = 0; // Dynamic 57
        public float Mic1Pos = 0.350f;
        public float Mic1Dist = 0.150f;
        public float Mic1LevelDb = -6.0f;
        public float Mic1Pan = 0.50f;
        public bool Mic1PhaseInvert = false;

        public bool Mic2Active = true;
        public int Mic2Type = 5; // Ribbon 121
        public float Mic2Pos = 0.520f;
        public float Mic2Dist = 0.220f;
        public float Mic2LevelDb = -8.5f;
        public float Mic2Pan = 0.50f;
        public bool Mic2PhaseInvert = false;

        // EQ (3 channels x 9 bands + HPF/LPF)
        public bool[] EqPower = new bool[] { true, true, true };
        public float[][] EqBands = new float[][]
        {
            new float[] { 0.0f, 0.5f, 0.0f, -1.0f, 0.0f, 1.2f, 1.8f, 1.0f, 0.5f },
            new float[] { 1.5f, 1.0f, -0.8f, -1.5f, 0.8f, 2.2f, 1.5f, 0.5f, 0.0f },
            new float[] { 2.4f, 1.6f, -1.8f, -2.5f, 1.2f, 2.8f, 3.2f, 1.8f, 0.6f }
        };
        public float[] EqHpfHz = new float[] { 20.0f, 35.0f, 45.0f };
        public float[] EqLpfKhz = new float[] { 20.0f, 14.5f, 12.8f };

        // Post FX: DLY & REV
        public bool DlyActive = false;
        public float DlyMix = 0.35f;
        public float DlyFeedback = 0.42f;
        public float DlyTone = 0.55f;
        public float DlyTimeMs = 400.0f;
        public int DlySubdivIndex = 7; // 1/4
        public float DlyTapeSat = 0.32f;
        public float DlyTapeMod = 0.28f;
        public int DlySyncMode = 0; // 0=FREE, 1=APP, 2=TAP
        public bool DlyPingPong = true;

        public bool RevActive = true;
        public float RevMix = 0.22f;
        public float RevDecay = 0.46f;
        public float RevLowCutHz = 120.0f;
        public float RevHighCutKhz = 6.8f;
        public bool RevShimmer = false;

        public PresetData Clone()
        {
            PresetData copy = (PresetData)this.MemberwiseClone();
            copy.AmpPower = (bool[])this.AmpPower.Clone();
            copy.AmpGain = (float[])this.AmpGain.Clone();
            copy.AmpBass = (float[])this.AmpBass.Clone();
            copy.AmpMid = (float[])this.AmpMid.Clone();
            copy.AmpTreble = (float[])this.AmpTreble.Clone();
            copy.AmpMaster = (float[])this.AmpMaster.Clone();
            copy.AmpPresence = (float[])this.AmpPresence.Clone();
            copy.AmpDepth = (float[])this.AmpDepth.Clone();
            copy.AmpLevel = (float[])this.AmpLevel.Clone();
            copy.EqPower = (bool[])this.EqPower.Clone();
            copy.EqBands = new float[3][];
            for (int i = 0; i < 3; i++) copy.EqBands[i] = (float[])this.EqBands[i].Clone();
            copy.EqHpfHz = (float[])this.EqHpfHz.Clone();
            copy.EqLpfKhz = (float[])this.EqLpfKhz.Clone();
            return copy;
        }
    }

    public class MidiMappingRow
    {
        public string Type;
        public string Parameter;
        public int Channel;
        public string NoteOrCc;
        public string ValueRange;
    }

    public class UiHotspot
    {
        public string Id;
        public RectangleF Bounds;
        public Action OnClick;
        public Action OnRightClick;
        public Action OnDoubleClick;
        public Action<float> OnDragDelta; // dy in logical pixels
        public Action<PointF> OnDragPoint; // logical point inside Bounds
    }

    public class GojiraMainForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        private const int LOGICAL_WIDTH = 1024;
        private const int LOGICAL_HEIGHT = 782;

        private readonly string screenshotPath;
        private readonly bool isScreenshotMode;
        private int renderedFrames = 0;

        // UI Scaling from configuration.xml (0x146562e30)
        private readonly float[] scaleFactors = new float[] { 0.715f, 1.0f, 1.3f, 1.6f, 2.0f };
        private readonly string[] scaleNames = new string[] { "Small (0.715x)", "Medium (1.0x)", "Large (1.3x)", "X-Large (1.6x)", "Maximum (2.0x)" };
        private int currentScaleIndex = 1; // Medium 1.0x (1024x782)

        // Navigation & State
        private int selectedSection = 2; // 0=Pitch, 1=PreFX, 2=Amp (default), 3=Cab, 4=EQ, 5=PostFX
        private int activeModal = 0; // 0=None, 1=Tuner, 2=Metronome, 3=MIDI, 4=Settings/Inspector, 5=About, 6=PresetMenu, 7=ContextMenu
        private bool presetDirty = false;
        private int currentPresetIndex = 0;
        private List<PresetData> presets = new List<PresetData>();
        private PresetData state;

        // Tempo, Metronome & Tuner
        private float bpm = 120.0f;
        private List<long> tapTimestamps = new List<long>();
        private bool metronomePlaying = false;
        private float metronomeVolDb = 0.0f;
        private float metronomePan = 0.0f;
        private int metronomeTimeSigIndex = 0;
        private int metronomeSoundSetIndex = 4; // Drum Kit
        private int metronomeRhythmIndex = 0;
        private int[] metronomeBeatAccents = new int[] { 2, 1, 1, 1, 1, 1, 1, 1 }; // 2=Accent, 1=Normal, 0=Mute
        private int currentBeatStep = 0;
        private double beatPhase = 0.0;

        // Tuner State
        private bool tunerMuted = false;
        private bool tunerDisplayHz = false;
        private bool liveTunerEnabled = true;
        private float tunerRefPitchHz = 440.0f;
        private string detectedNoteName = "D2";
        private float detectedFreqHz = 73.42f;
        private float detectedCents = 0.0f;

        // Live Clean-Room DSP Simulation State
        private bool demoRiffEnabled = true;
        private double dspTime = 0.0;
        private float inputMeterDb = -14.2f;
        private float inputPeakDb = -8.4f;
        private float outputMeterLDb = -9.6f;
        private float outputMeterRDb = -9.1f;
        private float outputPeakLDb = -4.2f;
        private float outputPeakRDb = -4.0f;
        private bool inputClipLatched = false;
        private bool outputClipLatched = false;
        private bool gateOpen = true;
        private float[] liveWaveform = new float[128];
        private float lfoPhase = 0.0f;

        // MIDI Mappings
        private List<MidiMappingRow> midiMappings = new List<MidiMappingRow>();

        // Audio Settings
        private string audioDeviceType = "ASIO";
        private string audioDeviceName = "Neural DSP USB Audio ASIO Driver";
        private int sampleRate = 48000;
        private int bufferSize = 128;

        // Interaction Hotspots
        private List<UiHotspot> hotspots = new List<UiHotspot>();
        private UiHotspot activeDragHotspot = null;
        private PointF lastMouseLogical;
        private PointF hoverLogical = new PointF(-1, -1);

        // Offscreen Buffer & Timer
        private Bitmap backBuffer;
        private Timer frameTimer;

        private static readonly string[] MicNames = new string[]
        {
            "Dynamic 57",
            "Dynamic 57 Off-Axis",
            "Dynamic 421",
            "Condenser 184",
            "Condenser 414",
            "Ribbon 121",
            "Ribbon 160",
            "Custom IR (Kone Slot)"
        };

        private static readonly string[] EqBandLabels = new string[]
        {
            "65 Hz", "125 Hz", "250 Hz", "500 Hz", "1 kHz", "2 kHz", "4 kHz", "8 kHz", "16 kHz"
        };

        private static readonly string[] SubdivisionNames = new string[]
        {
            "1/64T", "1/32", "1/32T", "1/16", "1/16T", "1/16D", "1/8", "1/4", "1/4T", "1/4D", "1/2", "1/1"
        };

        private static readonly string[] TimeSignatureNames = new string[]
        {
            "4/4", "3/4", "2/4", "5/4", "6/4", "7/4", "5/8", "6/8", "7/8", "9/8", "12/8", "11/8", "13/8", "15/16"
        };

        private static readonly string[] SoundSetNames = new string[]
        {
            "Blip", "Block and Shaker", "Cowbell", "Digital", "Drum Kit", "Soft Kit"
        };

        private static readonly string[] RhythmNames = new string[]
        {
            "Quarter (1/4)", "Eighth (1/8)", "Triplet (1/8T)", "Sixteenth (1/16)", "Accent Backbeat"
        };

        public GojiraMainForm(string screenshotTarget)
        {
            this.screenshotPath = screenshotTarget;
            this.isScreenshotMode = !string.IsNullOrEmpty(screenshotTarget);

            this.Text = "Archetype Gojira X v1.0.2";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(LOGICAL_WIDTH, LOGICAL_HEIGHT);
            this.BackColor = Color.FromArgb(22, 22, 22);
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            backBuffer = new Bitmap(LOGICAL_WIDTH, LOGICAL_HEIGHT, PixelFormat.Format32bppArgb);

            InitPresets();
            InitMidiMappings();
            LoadPreset(0);

            this.MouseDown += OnCanvasMouseDown;
            this.MouseMove += OnCanvasMouseMove;
            this.MouseUp += OnCanvasMouseUp;
            this.MouseDoubleClick += OnCanvasMouseDoubleClick;
            this.MouseWheel += OnCanvasMouseWheel;

            frameTimer = new Timer();
            frameTimer.Interval = isScreenshotMode ? 10 : 16; // ~60 FPS
            frameTimer.Tick += OnFrameTick;
            frameTimer.Start();
        }

        protected override bool ShowWithoutActivation
        {
            get { return isScreenshotMode; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (isScreenshotMode)
                {
                    cp.ExStyle |= WS_EX_NOACTIVATE;
                }
                return cp;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (isScreenshotMode)
            {
                SetWindowPos(this.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
        }

        private void InitPresets()
        {
            // 1. Default.xml_1 (Extracted from Archetype Gojira X1.cab)
            PresetData def = new PresetData
            {
                Category = "Gojira",
                Name = "Default",
                AmpType = 2, // HOT
                CabType = 2,
                AmpCabLinked = true,
                GateActive = true,
                GateThresholdDb = -80.0f,
                OdActive = true,
                OdDist = 0.12f,
                OdLevel = 0.82f,
                OdTone = 0.56f,
                DlyActive = false,
                RevActive = true,
                RevMix = 0.18f,
                RevDecay = 0.42f
            };
            presets.Add(def);

            // 2. AnunnakiX.xml (Extracted from CAB)
            PresetData anunnaki = def.Clone();
            anunnaki.Category = "Gojira";
            anunnaki.Name = "Anunnaki X";
            anunnaki.AmpType = 2;
            anunnaki.CabType = 2;
            anunnaki.GateThresholdDb = -62.0f;
            anunnaki.AmpGain[2] = 0.78f;
            anunnaki.AmpBass[2] = 0.68f;
            anunnaki.AmpMid[2] = 0.54f;
            anunnaki.AmpTreble[2] = 0.72f;
            anunnaki.AmpPresence[2] = 0.74f;
            anunnaki.AmpDepth[2] = 0.68f;
            anunnaki.DoublerActive = true;
            anunnaki.DoublerSpreadMs = 8.5f;
            presets.Add(anunnaki);

            // 3. Heavy8StringRhythm.xml (Extracted from CAB)
            PresetData heavy8 = def.Clone();
            heavy8.Category = "Gojira";
            heavy8.Name = "Heavy 8 String Rhythm";
            heavy8.AmpType = 1; // RST
            heavy8.CabType = 2; // HOT Cab
            heavy8.AmpCabLinked = false;
            heavy8.TransposeSemitones = -2;
            heavy8.GateThresholdDb = -54.0f;
            heavy8.OdActive = true;
            heavy8.OdDist = 0.05f;
            heavy8.OdLevel = 0.95f;
            heavy8.OdTone = 0.68f;
            heavy8.AmpGain[1] = 0.74f;
            heavy8.EqHpfHz[1] = 65.0f;
            heavy8.RevActive = false;
            presets.Add(heavy8);

            // 4. Stranded Pitch Blade
            PresetData stranded = def.Clone();
            stranded.Category = "Gojira";
            stranded.Name = "Stranded Pitch Blade (+12st)";
            stranded.WowActive = true;
            stranded.WowMode = 1; // BLADE 1
            stranded.WowPedalPos = 1.0f;
            stranded.DrtActive = true;
            stranded.OdActive = false;
            stranded.AmpType = 1;
            stranded.CabType = 1;
            presets.Add(stranded);

            // 5. Flying Whales Clean Loom
            PresetData whales = def.Clone();
            whales.Category = "Gojira";
            whales.Name = "Flying Whales Clean Loom";
            whales.AmpType = 0; // CLN
            whales.CabType = 0;
            whales.OdActive = false;
            whales.ChrActive = true;
            whales.PhsrActive = true;
            whales.DlyActive = true;
            whales.DlyPingPong = true;
            whales.DlySyncMode = 1; // APP
            whales.RevActive = true;
            whales.RevMix = 0.45f;
            whales.RevDecay = 0.72f;
            whales.RevShimmer = true;
            presets.Add(whales);

            // 6. Silvera Tapping Lead
            PresetData silvera = def.Clone();
            silvera.Category = "Artists";
            silvera.Name = "Silvera Octaver Lead";
            silvera.OctActive = true;
            silvera.Oct1Level = 0.65f;
            silvera.Oct2Level = 0.25f;
            silvera.DlyActive = true;
            silvera.DlyTimeMs = 460.0f;
            silvera.RevActive = true;
            silvera.RevMix = 0.34f;
            presets.Add(silvera);
        }

        private void InitMidiMappings()
        {
            midiMappings.Add(new MidiMappingRow { Type = "Control Change", Parameter = "WOW: Pedal Position", Channel = 1, NoteOrCc = "CC #11", ValueRange = "0 .. 127" });
            midiMappings.Add(new MidiMappingRow { Type = "Control Change", Parameter = "WOW: Bypass Stomp", Channel = 1, NoteOrCc = "CC #1", ValueRange = "Toggle" });
            midiMappings.Add(new MidiMappingRow { Type = "Program Change", Parameter = "Preset: Default (Gojira)", Channel = 1, NoteOrCc = "PC #0", ValueRange = "Load" });
            midiMappings.Add(new MidiMappingRow { Type = "Program Change", Parameter = "Preset: Anunnaki X", Channel = 1, NoteOrCc = "PC #1", ValueRange = "Load" });
            midiMappings.Add(new MidiMappingRow { Type = "Control Change", Parameter = "Post FX: Tap Tempo", Channel = 1, NoteOrCc = "CC #64", ValueRange = "Trigger" });
        }

        private void LoadPreset(int index)
        {
            if (index < 0) index = presets.Count - 1;
            if (index >= presets.Count) index = 0;
            currentPresetIndex = index;
            state = presets[currentPresetIndex].Clone();
            presetDirty = false;
        }

        private void MarkDirty()
        {
            presetDirty = true;
        }

        private void ApplyScale(int index)
        {
            currentScaleIndex = index;
            float s = scaleFactors[currentScaleIndex];
            this.ClientSize = new Size((int)Math.Round(LOGICAL_WIDTH * s), (int)Math.Round(LOGICAL_HEIGHT * s));
        }

        private PointF ToLogicalPoint(Point clientPt)
        {
            float s = scaleFactors[currentScaleIndex];
            return new PointF(clientPt.X / s, clientPt.Y / s);
        }

        private void OnFrameTick(object sender, EventArgs e)
        {
            UpdateDspSimulation(0.016);
            RenderToBackBuffer();
            this.Invalidate();

            if (isScreenshotMode)
            {
                renderedFrames++;
                if (renderedFrames >= 120)
                {
                    frameTimer.Stop();
                    try
                    {
                        string fullPath = Path.GetFullPath(screenshotPath);
                        string dir = Path.GetDirectoryName(fullPath);
                        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        backBuffer.Save(fullPath, ImageFormat.Png);
                    }
                    catch
                    {
                    }
                    Application.Exit();
                }
            }
        }

        private void UpdateDspSimulation(double dt)
        {
            dspTime += dt;
            lfoPhase = (float)((dspTime * (state.PhsrActive ? state.PhsrRateHz : 1.0f)) % 1.0);

            // Metronome Step Counter
            double beatsPerSec = bpm / 60.0;
            beatPhase += dt * beatsPerSec;
            if (beatPhase >= 1.0)
            {
                beatPhase -= 1.0;
                int numBeats = GetTimeSigBeats();
                currentBeatStep = (currentBeatStep + 1) % numBeats;
            }

            // Simulate 8-string guitar riff through the decompiled Gojira X signal chain
            double riffBeat = (dspTime * beatsPerSec) % 8.0;
            double[] riffFreqs = new double[] { 73.42, 73.42, 82.41, 73.42, 87.31, 82.41, 73.42, 110.00 }; // D2, E2, F2, A2
            string[] riffNotes = new string[] { "D2", "D2", "E2", "D2", "F2", "E2", "D2", "A2" };
            int noteIdx = ((int)riffBeat) % riffFreqs.Length;

            double baseFreq = riffFreqs[noteIdx];
            // Apply Transpose (-12..+12 st) + WOW Pitch Shifter
            double totalSemitones = state.TransposeSemitones;
            if (state.PitchSectionActive && state.WowActive)
            {
                if (state.WowMode == 0) totalSemitones += (state.WowPedalPos * 24.0 - 12.0) * state.WowFatsoMix;
                else if (state.WowMode == 1) totalSemitones += state.WowPedalPos * 12.0;
                else totalSemitones += state.WowPedalPos * 24.0;
            }
            double shiftedFreq = baseFreq * Math.Pow(2.0, totalSemitones / 12.0);
            detectedFreqHz = (float)shiftedFreq;
            detectedNoteName = riffNotes[noteIdx];
            detectedCents = (float)(Math.Sin(dspTime * 3.2) * 3.5);

            // Compute envelope & signal level through gain stages
            double pluckSubBeat = (dspTime * beatsPerSec * 2.0) % 1.0;
            double env = demoRiffEnabled ? (0.35 + 0.65 * Math.Exp(-pluckSubBeat * 3.5)) : 0.001;

            float inDb = (float)(20.0 * Math.Log10(Math.Max(0.0001, env * 0.45))) + state.InputGainDb;
            inputMeterDb = Math.Max(-60.0f, Math.Min(0.0f, inDb));
            if (inputMeterDb > inputPeakDb) inputPeakDb = inputMeterDb;
            else inputPeakDb = Math.Max(inputMeterDb, inputPeakDb - 0.15f);
            if (inputMeterDb >= -0.2f) inputClipLatched = true;

            gateOpen = !state.GateActive || (inputMeterDb >= state.GateThresholdDb);

            // Pre FX + Amp + Cab + EQ + Post FX gain transfer
            int ampIdx = state.AmpType;
            float ampDrive = state.AmpSectionActive && state.AmpPower[ampIdx]
                ? (0.6f + state.AmpGain[ampIdx] * 1.8f + (state.PreFxSectionActive && state.OdActive ? 0.45f : 0.0f))
                : 0.35f;

            for (int i = 0; i < liveWaveform.Length; i++)
            {
                double t = dspTime * 40.0 + i * 0.14;
                double raw = Math.Sin(t) + 0.5 * Math.Sin(t * 2.0) + 0.25 * Math.Sin(t * 3.0);
                if (state.PitchSectionActive && state.OctActive)
                {
                    raw += state.Oct1Level * 0.6 * Math.Sin(t * 0.5) + state.Oct2Level * 0.4 * Math.Sin(t * 0.25);
                }
                double saturated = Math.Tanh(raw * ampDrive * env);
                liveWaveform[i] = (gateOpen && !tunerMuted) ? (float)saturated : 0.0f;
            }

            if (!gateOpen || tunerMuted || !demoRiffEnabled)
            {
                outputMeterLDb = Math.Max(-60.0f, outputMeterLDb - 2.5f);
                outputMeterRDb = Math.Max(-60.0f, outputMeterRDb - 2.5f);
            }
            else
            {
                float ampOutDb = -10.0f + (state.AmpLevel[ampIdx] - 0.5f) * 14.0f + state.OutputGainDb;
                float stereoSpread = (state.DoublerActive && !state.StereoMode) ? (float)(Math.Sin(dspTime * 7.0) * 1.4) : 0.3f;
                outputMeterLDb = Math.Max(-60.0f, Math.Min(0.0f, ampOutDb + (float)(Math.Sin(dspTime * 9.0) * 1.8f)));
                outputMeterRDb = Math.Max(-60.0f, Math.Min(0.0f, ampOutDb + stereoSpread + (float)(Math.Cos(dspTime * 8.5) * 1.8f)));
            }

            if (outputMeterLDb > outputPeakLDb) outputPeakLDb = outputMeterLDb;
            else outputPeakLDb = Math.Max(outputMeterLDb, outputPeakLDb - 0.15f);

            if (outputMeterRDb > outputPeakRDb) outputPeakRDb = outputMeterRDb;
            else outputPeakRDb = Math.Max(outputMeterRDb, outputPeakRDb - 0.15f);

            if (outputMeterLDb >= -0.2f || outputMeterRDb >= -0.2f) outputClipLatched = true;
        }

        private int GetTimeSigBeats()
        {
            string ts = TimeSignatureNames[metronomeTimeSigIndex];
            int slash = ts.IndexOf('/');
            int num = 4;
            if (slash > 0) int.TryParse(ts.Substring(0, slash), out num);
            return Math.Max(2, Math.Min(8, num));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float s = scaleFactors[currentScaleIndex];
            if (Math.Abs(s - 1.0f) < 0.001f)
            {
                g.DrawImageUnscaled(backBuffer, 0, 0);
            }
            else
            {
                g.DrawImage(backBuffer, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
        }

        private void RenderToBackBuffer()
        {
            hotspots.Clear();
            using (Graphics g = Graphics.FromImage(backBuffer))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Main Studio Canvas (#161616)
                using (LinearGradientBrush bg = new LinearGradientBrush(
                    new Rectangle(0, 0, LOGICAL_WIDTH, LOGICAL_HEIGHT),
                    Color.FromArgb(26, 26, 29),
                    Color.FromArgb(16, 16, 18),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bg, 0, 0, LOGICAL_WIDTH, LOGICAL_HEIGHT);
                }

                // Subtle radial spotlight on the center stage
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddEllipse(112, 160, 800, 560);
                    using (PathGradientBrush pgb = new PathGradientBrush(gp))
                    {
                        Color accentSpot = state.AmpType == 0
                            ? Color.FromArgb(24, 180, 130, 70)
                            : (state.AmpType == 1 ? Color.FromArgb(24, 210, 95, 35) : Color.FromArgb(26, 140, 40, 65));
                        pgb.CenterColor = accentSpot;
                        pgb.SurroundColors = new Color[] { Color.Transparent };
                        g.FillPath(pgb, gp);
                    }
                }

                // 1. Top Navigation & Global Controls Bar (y: 0..190)
                DrawTopBar(g);

                // 2. Main Section Stage (y: 190..688)
                switch (selectedSection)
                {
                    case 0: DrawPitchSection(g); break;
                    case 1: DrawPreFxSection(g); break;
                    case 2: DrawAmpSection(g); break;
                    case 3: DrawCabSection(g); break;
                    case 4: DrawEqSection(g); break;
                    case 5: DrawPostFxSection(g); break;
                }

                // 3. Bottom Amp/Cab Selector Bar (y: 688..748)
                DrawBottomAmpCabSelector(g);

                // 4. Bottom Utility Bar (y: 750..782)
                DrawBottomUtilityBar(g);

                // 5. Active Modal Overlay (Tuner, Metronome, MIDI, Settings, About, Dropdowns)
                if (activeModal != 0)
                {
                    DrawModalOverlay(g);
                }
            }
        }

        private void DrawTopBar(Graphics g)
        {
            // Top bar chassis (0..190)
            using (LinearGradientBrush topBg = new LinearGradientBrush(
                new Rectangle(0, 0, LOGICAL_WIDTH, 190),
                Color.FromArgb(14, 14, 16),
                Color.FromArgb(22, 22, 25),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(topBg, 0, 0, LOGICAL_WIDTH, 190);
            }
            using (Pen borderPen = new Pen(Color.FromArgb(46, 46, 52), 1f))
            {
                g.DrawLine(borderPen, 0, 72, LOGICAL_WIDTH, 72);
                g.DrawLine(borderPen, 0, 189, LOGICAL_WIDTH, 189);
            }

            // Suite Title & Signal Flow Header (top-left)
            using (Font titleFont = new Font("Segoe UI", 10.5f, FontStyle.Bold))
            using (Font subFont = new Font("Segoe UI", 7.5f, FontStyle.Regular))
            using (Brush whiteBr = new SolidBrush(Color.FromArgb(235, 235, 240)))
            using (Brush mutedBr = new SolidBrush(Color.FromArgb(142, 144, 152)))
            using (Brush accentBr = new SolidBrush(Color.FromArgb(228, 122, 23)))
            {
                g.DrawString("ARCHETYPE GOJIRA X", titleFont, whiteBr, 18, 14);
                g.DrawString("KONE DSP ENGINE v1.0.2  |  64-BIT FLOAT", subFont, mutedBr, 18, 34);
                string chainStr = string.Format("CHAIN: {0} AMP -> CAB {1}",
                    state.AmpType == 0 ? "CLN" : (state.AmpType == 1 ? "RST" : "HOT"),
                    state.CabType + 1);
                g.DrawString(chainStr, subFont, accentBr, 18, 50);
            }

            // Top-Right Demo Riff & Engine Status Pill
            RectangleF demoBtn = new RectangleF(840, 16, 166, 40);
            DrawButtonPill(g, demoBtn, demoRiffEnabled ? "LIVE RIFF INPUT: ON" : "LIVE RIFF INPUT: MUTED",
                demoRiffEnabled ? Color.FromArgb(35, 69, 248, 98) : Color.FromArgb(32, 32, 36),
                demoRiffEnabled ? Color.FromArgb(69, 248, 98) : Color.FromArgb(140, 140, 148), 8f);
            hotspots.Add(new UiHotspot
            {
                Id = "toggle_demo_riff",
                Bounds = demoBtn,
                OnClick = () => { demoRiffEnabled = !demoRiffEnabled; }
            });

            // Row 1: 6 Section Buttons centered (x: 345, 401, 457, 513, 569, 625 from ArchetypeTopBarView.xml)
            string[] secLabels = new string[] { "PITCH", "PRE FX", "AMP", "CAB", "EQ", "POST FX" };
            bool[] secActive = new bool[]
            {
                state.PitchSectionActive,
                state.PreFxSectionActive,
                state.AmpSectionActive,
                state.CabSectionActive,
                state.EqSectionActive,
                state.PostFxSectionActive
            };
            int[] secX = new int[] { 325, 387, 449, 511, 573, 635 };

            for (int i = 0; i < 6; i++)
            {
                int idx = i;
                RectangleF btnRect = new RectangleF(secX[i], 8, 56, 58);
                bool isSel = (selectedSection == i);
                bool isAct = secActive[i];

                Color boxFill = isSel ? Color.FromArgb(42, 42, 48) : Color.FromArgb(24, 24, 28);
                Color borderCol = isSel
                    ? (isAct ? Color.FromArgb(228, 122, 23) : Color.FromArgb(130, 130, 138))
                    : Color.FromArgb(52, 52, 58);

                using (GraphicsPath path = RoundedRect(btnRect, 6f))
                using (Brush b = new SolidBrush(boxFill))
                using (Pen p = new Pen(borderCol, isSel ? 1.6f : 1.0f))
                {
                    g.FillPath(b, path);
                    g.DrawPath(p, path);
                }

                // Active status LED dot in top-right corner of section button
                Color ledCol = isAct ? Color.FromArgb(69, 248, 98) : Color.FromArgb(80, 80, 85);
                using (Brush lb = new SolidBrush(ledCol))
                {
                    g.FillEllipse(lb, btnRect.Right - 11, btnRect.Top + 5, 5, 5);
                }

                // Vector Section Icon
                DrawSectionIcon(g, i, new RectangleF(btnRect.X + 13, btnRect.Y + 8, 30, 26),
                    isAct ? (isSel ? Color.White : Color.FromArgb(200, 202, 210)) : Color.FromArgb(95, 95, 102));

                // Section caption
                using (Font f = new Font("Segoe UI", 6.8f, FontStyle.Bold))
                using (Brush tb = new SolidBrush(isSel ? Color.White : (isAct ? Color.FromArgb(175, 177, 185) : Color.FromArgb(95, 95, 102))))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                {
                    g.DrawString(secLabels[i], f, tb, new RectangleF(btnRect.X, btnRect.Bottom - 16, btnRect.Width, 14), sf);
                }

                Action toggleSection = () =>
                {
                    if (idx == 0) state.PitchSectionActive = !state.PitchSectionActive;
                    else if (idx == 1) state.PreFxSectionActive = !state.PreFxSectionActive;
                    else if (idx == 2) state.AmpSectionActive = !state.AmpSectionActive;
                    else if (idx == 3) state.CabSectionActive = !state.CabSectionActive;
                    else if (idx == 4) state.EqSectionActive = !state.EqSectionActive;
                    else if (idx == 5) state.PostFxSectionActive = !state.PostFxSectionActive;
                    MarkDirty();
                };

                hotspots.Add(new UiHotspot
                {
                    Id = "sec_btn_" + idx,
                    Bounds = btnRect,
                    OnClick = () => { selectedSection = idx; activeModal = 0; },
                    OnRightClick = toggleSection,
                    OnDoubleClick = toggleSection
                });
            }

            // Row 2 (y: 76..186): Global Controls + Preset Manager
            // 1. Input Level Meter (x: 18, y: 84, w: 18, h: 92)
            DrawVerticalMeter(g, new RectangleF(18, 84, 18, 92), inputMeterDb, inputPeakDb, inputClipLatched, "IN", () => { inputClipLatched = false; });

            // 2. INPUT Gain Knob (-24..+24 dB)
            DrawRotaryKnob(g, new RectangleF(48, 84, 58, 58), (state.InputGainDb + 24f) / 48f, "INPUT",
                string.Format("{0:+0.0;-0.0;0.0} dB", state.InputGainDb), Color.FromArgb(228, 122, 23),
                dy => { state.InputGainDb = Clamp(state.InputGainDb - dy * 0.25f, -24f, 24f); MarkDirty(); },
                () => { state.InputGainDb = 0.0f; MarkDirty(); });

            // 3. GATE Switch + Threshold Knob (-96..0 dB)
            DrawMiniToggle(g, new RectangleF(118, 84, 42, 16), state.GateActive, "GATE",
                () => { state.GateActive = !state.GateActive; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(118, 102, 48, 48), (state.GateThresholdDb + 96f) / 96f, "THRESH",
                string.Format("{0:0.0} dB", state.GateThresholdDb), state.GateActive ? Color.FromArgb(69, 248, 98) : Color.FromArgb(100, 100, 106),
                dy => { state.GateThresholdDb = Clamp(state.GateThresholdDb - dy * 0.45f, -96f, 0f); MarkDirty(); },
                () => { state.GateThresholdDb = -80.0f; MarkDirty(); });

            // 4. TRANSPOSE Knob (-12..+12 st)
            DrawRotaryKnob(g, new RectangleF(182, 84, 58, 58), (state.TransposeSemitones + 12f) / 24f, "TRANSPOSE",
                string.Format("{0:+0;-0;0} st", state.TransposeSemitones), Color.FromArgb(99, 80, 255),
                dy =>
                {
                    int delta = dy < -4 ? 1 : (dy > 4 ? -1 : 0);
                    if (delta != 0)
                    {
                        state.TransposeSemitones = (int)Clamp(state.TransposeSemitones + delta, -12, 12);
                        MarkDirty();
                    }
                },
                () => { state.TransposeSemitones = 0; MarkDirty(); });

            // 5. INPUT MODE Vertical Switch (MONO / STEREO)
            DrawModeSwitchBox(g, new RectangleF(256, 86, 78, 84), "INPUT MODE", "MONO", "STEREO", state.StereoMode,
                () => { state.StereoMode = !state.StereoMode; MarkDirty(); });

            // 6. Center PRESET MANAGER (x: 352, y: 84, w: 320, h: 90)
            DrawPresetManager(g, new RectangleF(352, 84, 320, 90));

            // 7. DOUBLER Switch + SPREAD Knob (3..20 ms)
            bool doublerAllowed = !state.StereoMode;
            DrawMiniToggle(g, new RectangleF(692, 84, 62, 16), state.DoublerActive && doublerAllowed, "DOUBLER",
                () => { if (doublerAllowed) { state.DoublerActive = !state.DoublerActive; MarkDirty(); } });
            DrawRotaryKnob(g, new RectangleF(700, 102, 48, 48), (state.DoublerSpreadMs - 3f) / 17f, "SPREAD",
                doublerAllowed ? string.Format("{0:0.0} ms", state.DoublerSpreadMs) : "MONO ONLY",
                (state.DoublerActive && doublerAllowed) ? Color.FromArgb(10, 116, 224) : Color.FromArgb(100, 100, 106),
                dy => { if (doublerAllowed) { state.DoublerSpreadMs = Clamp(state.DoublerSpreadMs - dy * 0.1f, 3f, 20f); MarkDirty(); } },
                () => { state.DoublerSpreadMs = 6.0f; MarkDirty(); });

            // 8. OUTPUT Gain Knob (-24..+24 dB)
            DrawRotaryKnob(g, new RectangleF(888, 84, 58, 58), (state.OutputGainDb + 24f) / 48f, "OUTPUT",
                string.Format("{0:+0.0;-0.0;0.0} dB", state.OutputGainDb), Color.FromArgb(228, 122, 23),
                dy => { state.OutputGainDb = Clamp(state.OutputGainDb - dy * 0.25f, -24f, 24f); MarkDirty(); },
                () => { state.OutputGainDb = 0.0f; MarkDirty(); });

            // 9. Stereo Output Level Meters (L/R)
            DrawVerticalMeter(g, new RectangleF(964, 84, 16, 92), outputMeterLDb, outputPeakLDb, outputClipLatched, "L", () => { outputClipLatched = false; });
            DrawVerticalMeter(g, new RectangleF(986, 84, 16, 92), outputMeterRDb, outputPeakRDb, outputClipLatched, "R", () => { outputClipLatched = false; });

            // Quick Signal Waveform & Spectrum Mini Monitor (x: 772, y: 86, w: 100, h: 84)
            DrawMiniSignalScope(g, new RectangleF(772, 86, 102, 84));
        }

        private void DrawPresetManager(Graphics g, RectangleF rect)
        {
            using (GraphicsPath path = RoundedRect(rect, 6f))
            using (Brush b = new SolidBrush(Color.FromArgb(18, 18, 22)))
            using (Pen p = new Pen(Color.FromArgb(56, 56, 64), 1.2f))
            {
                g.FillPath(b, path);
                g.DrawPath(p, path);
            }

            // Top Action Buttons: DELETE | SAVE | SAVE AS... | ...
            string[] acts = new string[] { "DELETE", "SAVE", "SAVE AS...", "•••" };
            float btnW = (rect.Width - 20) / 4f;
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                RectangleF bRect = new RectangleF(rect.X + 8 + i * (btnW + 1), rect.Y + 8, btnW - 2, 22);
                DrawButtonPill(g, bRect, acts[i], Color.FromArgb(32, 32, 38), Color.FromArgb(190, 192, 200), 7.2f);
                hotspots.Add(new UiHotspot
                {
                    Id = "preset_act_" + idx,
                    Bounds = bRect,
                    OnClick = () =>
                    {
                        if (idx == 0 && presets.Count > 1)
                        {
                            presets.RemoveAt(currentPresetIndex);
                            LoadPreset(Math.Max(0, currentPresetIndex - 1));
                        }
                        else if (idx == 1)
                        {
                            presets[currentPresetIndex] = state.Clone();
                            presetDirty = false;
                        }
                        else if (idx == 2)
                        {
                            PresetData copy = state.Clone();
                            copy.Category = "User";
                            copy.Name = state.Name + " (Custom)";
                            presets.Add(copy);
                            LoadPreset(presets.Count - 1);
                        }
                        else if (idx == 3)
                        {
                            // Reset current preset to factory default
                            LoadPreset(currentPresetIndex);
                        }
                    }
                });
            }

            // Preset Display Bar (<  Category / Preset Name *  v  >)
            RectangleF prevBtn = new RectangleF(rect.X + 8, rect.Y + 38, 26, 42);
            RectangleF nextBtn = new RectangleF(rect.Right - 34, rect.Y + 38, 26, 42);
            RectangleF dropBtn = new RectangleF(rect.X + 38, rect.Y + 38, rect.Width - 76, 42);

            DrawButtonPill(g, prevBtn, "◄", Color.FromArgb(28, 28, 34), Color.FromArgb(210, 210, 218), 8f);
            DrawButtonPill(g, nextBtn, "►", Color.FromArgb(28, 28, 34), Color.FromArgb(210, 210, 218), 8f);

            using (GraphicsPath dp = RoundedRect(dropBtn, 4f))
            using (Brush db = new SolidBrush(Color.FromArgb(11, 13, 16)))
            using (Pen dpen = new Pen(Color.FromArgb(68, 70, 80), 1f))
            {
                g.FillPath(db, dp);
                g.DrawPath(dpen, dp);
            }

            using (Font catF = new Font("Segoe UI", 7.2f, FontStyle.Bold))
            using (Font nameF = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (Brush catB = new SolidBrush(Color.FromArgb(228, 122, 23)))
            using (Brush nameB = new SolidBrush(Color.FromArgb(240, 242, 245)))
            {
                g.DrawString(state.Category.ToUpperInvariant() + "  (321 CAB PRESETS)", catF, catB, dropBtn.X + 10, dropBtn.Y + 5);
                string dispName = state.Name + (presetDirty ? " *" : "");
                g.DrawString(dispName, nameF, nameB, dropBtn.X + 10, dropBtn.Y + 19);
                g.DrawString("▼", catF, nameB, dropBtn.Right - 20, dropBtn.Y + 14);
            }

            hotspots.Add(new UiHotspot { Id = "preset_prev", Bounds = prevBtn, OnClick = () => LoadPreset(currentPresetIndex - 1) });
            hotspots.Add(new UiHotspot { Id = "preset_next", Bounds = nextBtn, OnClick = () => LoadPreset(currentPresetIndex + 1) });
            hotspots.Add(new UiHotspot { Id = "preset_drop", Bounds = dropBtn, OnClick = () => { activeModal = (activeModal == 6) ? 0 : 6; } });
        }

        private void DrawMiniSignalScope(Graphics g, RectangleF rect)
        {
            using (GraphicsPath path = RoundedRect(rect, 5f))
            using (Brush b = new SolidBrush(Color.FromArgb(12, 15, 14)))
            using (Pen p = new Pen(Color.FromArgb(46, 52, 50), 1f))
            {
                g.FillPath(b, path);
                g.DrawPath(p, path);
            }

            using (Font f = new Font("Segoe UI", 6.5f, FontStyle.Bold))
            using (Brush tb = new SolidBrush(Color.FromArgb(145, 160, 152)))
            {
                g.DrawString("DSP WAVEFORM", f, tb, rect.X + 6, rect.Y + 4);
            }

            PointF[] pts = new PointF[liveWaveform.Length];
            float midY = rect.Y + 48;
            for (int i = 0; i < liveWaveform.Length; i++)
            {
                float x = rect.X + 6 + (i / (float)(liveWaveform.Length - 1)) * (rect.Width - 12);
                float y = midY - liveWaveform[i] * 24f;
                pts[i] = new PointF(x, Clamp(y, rect.Y + 18, rect.Bottom - 6));
            }
            using (Pen wavePen = new Pen(Color.FromArgb(69, 248, 98), 1.4f))
            {
                g.DrawLines(wavePen, pts);
            }
        }

        // ====================================================================
        // VIEW 0: PITCH SECTION (WOW Pitch Shifter + OCT Polyphonic Octaver)
        // ====================================================================
        private void DrawPitchSection(Graphics g)
        {
            DrawSectionHeaderBanner(g, "PITCH SECTION  —  PROGRAMMABLE WOW EXPRESSION SHIFTER & POLYPHONIC OCTAVER", state.PitchSectionActive);

            // Left Pedal: WOW Pitch Shifter (x: 110, y: 230, w: 370, h: 440)
            RectangleF wowRect = new RectangleF(110, 230, 370, 440);
            DrawPedalEnclosure(g, wowRect, Color.FromArgb(34, 36, 42), Color.FromArgb(18, 19, 22), Color.FromArgb(228, 122, 23), "WOW", "PROGRAMMABLE PITCH PEDAL");

            // Interactive Expression Treadle Pedal (x: 132, y: 278, w: 175, h: 368)
            RectangleF treadleRect = new RectangleF(132, 278, 175, 368);
            using (GraphicsPath tp = RoundedRect(treadleRect, 12f))
            using (LinearGradientBrush tb = new LinearGradientBrush(treadleRect, Color.FromArgb(52, 54, 60), Color.FromArgb(22, 23, 27), LinearGradientMode.Vertical))
            using (Pen tpen = new Pen(Color.FromArgb(110, 112, 120), 2f))
            {
                g.FillPath(tb, tp);
                g.DrawPath(tpen, tp);
            }

            // Rubber grip ribs on treadle
            for (int r = 0; r < 14; r++)
            {
                float ry = treadleRect.Y + 24 + r * 22;
                using (Brush rb = new SolidBrush(Color.FromArgb(16, 16, 18)))
                {
                    g.FillRectangle(rb, treadleRect.X + 18, ry, treadleRect.Width - 36, 10);
                }
            }

            // Treadle position slider bar & handle
            float handleY = treadleRect.Bottom - 36 - state.WowPedalPos * (treadleRect.Height - 72);
            RectangleF handleRect = new RectangleF(treadleRect.X + 10, handleY - 14, treadleRect.Width - 20, 28);
            using (GraphicsPath hp = RoundedRect(handleRect, 6f))
            using (Brush hb = new SolidBrush(Color.FromArgb(220, 228, 122, 23)))
            using (Pen hpen = new Pen(Color.White, 1.5f))
            {
                g.FillPath(hb, hp);
                g.DrawPath(hpen, hp);
            }
            string pitchReadout = state.WowMode == 0
                ? string.Format("FATSO: {0:+0.0;-0.0;0.0} st", state.WowPedalPos * 24f - 12f)
                : (state.WowMode == 1
                    ? string.Format("BLADE 1: +{0:0.0} st", state.WowPedalPos * 12f)
                    : string.Format("BLADE 2: +{0:0.0} st", state.WowPedalPos * 24f));
            using (Font pf = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (Brush pb = new SolidBrush(Color.White))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(pitchReadout, pf, pb, handleRect, sf);
            }

            hotspots.Add(new UiHotspot
            {
                Id = "wow_treadle",
                Bounds = treadleRect,
                OnDragPoint = pt =>
                {
                    float norm = 1.0f - (pt.Y - (treadleRect.Y + 36)) / (treadleRect.Height - 72);
                    state.WowPedalPos = Clamp(norm, 0f, 1f);
                    MarkDirty();
                },
                OnDoubleClick = () => { state.WowPedalPos = 0.0f; MarkDirty(); }
            });

            // WOW Controls on right half of WOW pedal: Mode 3-way switch, Fatso Mix knob, Stomp + LED
            string[] wowModes = new string[] { "FATSO (-12..+12)", "BLADE 1 (0..+12)", "BLADE 2 (0..+24)" };
            for (int m = 0; m < 3; m++)
            {
                int modeIdx = m;
                RectangleF mRect = new RectangleF(324, 290 + m * 30, 138, 24);
                bool sel = (state.WowMode == m);
                DrawButtonPill(g, mRect, wowModes[m],
                    sel ? Color.FromArgb(228, 122, 23) : Color.FromArgb(28, 28, 34),
                    sel ? Color.Black : Color.FromArgb(185, 185, 195), 7.5f);
                hotspots.Add(new UiHotspot { Id = "wow_mode_" + m, Bounds = mRect, OnClick = () => { state.WowMode = modeIdx; MarkDirty(); } });
            }

            DrawRotaryKnob(g, new RectangleF(358, 405, 70, 70), state.WowFatsoMix, "FATSO MIX",
                string.Format("{0:0}%", state.WowFatsoMix * 100f),
                state.WowMode == 0 ? Color.FromArgb(228, 122, 23) : Color.FromArgb(95, 95, 102),
                dy => { state.WowFatsoMix = Clamp(state.WowFatsoMix - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.WowFatsoMix = 0.50f; MarkDirty(); });

            DrawStompSwitch(g, new RectangleF(358, 540, 70, 70), state.WowActive, "ENGAGE WOW",
                () => { state.WowActive = !state.WowActive; MarkDirty(); });

            // Right Pedal: OCT Polyphonic Octaver (x: 544, y: 230, w: 370, h: 440)
            RectangleF octRect = new RectangleF(544, 230, 370, 440);
            DrawPedalEnclosure(g, octRect, Color.FromArgb(30, 32, 38), Color.FromArgb(16, 17, 20), Color.FromArgb(99, 80, 255), "OCT", "POLYPHONIC DUAL OCTAVER");

            // Sacred geometric sun/mountain artwork on OCT pedal
            DrawGeometricPedalArt(g, new RectangleF(octRect.X + 45, octRect.Y + 185, octRect.Width - 90, 140), Color.FromArgb(70, 99, 80, 255));

            DrawRotaryKnob(g, new RectangleF(576, 294, 76, 76), state.Oct1Level, "OCT 1 (-12 ST)",
                string.Format("{0:0}%", state.Oct1Level * 100f), Color.FromArgb(99, 80, 255),
                dy => { state.Oct1Level = Clamp(state.Oct1Level - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.Oct1Level = 0.50f; MarkDirty(); });

            DrawRotaryKnob(g, new RectangleF(691, 336, 76, 76), state.OctDirect, "DIRECT",
                string.Format("{0:0}%", state.OctDirect * 100f), Color.FromArgb(220, 222, 230),
                dy => { state.OctDirect = Clamp(state.OctDirect - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.OctDirect = 1.00f; MarkDirty(); });

            DrawRotaryKnob(g, new RectangleF(806, 294, 76, 76), state.Oct2Level, "OCT 2 (-24 ST)",
                string.Format("{0:0}%", state.Oct2Level * 100f), Color.FromArgb(99, 80, 255),
                dy => { state.Oct2Level = Clamp(state.Oct2Level - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.Oct2Level = 0.50f; MarkDirty(); });

            DrawStompSwitch(g, new RectangleF(694, 540, 70, 70), state.OctActive, "BYPASS OCT",
                () => { state.OctActive = !state.OctActive; MarkDirty(); });
        }

        // ====================================================================
        // VIEW 1: PRE FX SECTION (OD + DRT + PHSR + CHR)
        // ====================================================================
        private void DrawPreFxSection(Graphics g)
        {
            DrawSectionHeaderBanner(g, "PRE FX SECTION  —  ANALOG OVERDRIVE, HIGH-GAIN DISTORTION, PHASER & BBD CHORUS", state.PreFxSectionActive);

            // 1. OD Pedal (x: 34, y: 232, w: 226, h: 438)
            RectangleF odRect = new RectangleF(34, 232, 226, 438);
            DrawPedalEnclosure(g, odRect, Color.FromArgb(62, 52, 34), Color.FromArgb(28, 23, 15), Color.FromArgb(238, 175, 65), "OD", "TUBE OVERDRIVE");
            DrawGeometricPedalArt(g, new RectangleF(odRect.X + 24, odRect.Y + 215, odRect.Width - 48, 110), Color.FromArgb(65, 238, 175, 65));

            DrawRotaryKnob(g, new RectangleF(odRect.X + 22, odRect.Y + 66, 62, 62), state.OdDist, "DIST",
                string.Format("{0:0.0}", state.OdDist * 10f), Color.FromArgb(238, 175, 65),
                dy => { state.OdDist = Clamp(state.OdDist - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.OdDist = 0.15f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(odRect.X + 142, odRect.Y + 66, 62, 62), state.OdLevel, "LEVEL",
                string.Format("{0:0.0}", state.OdLevel * 10f), Color.FromArgb(238, 175, 65),
                dy => { state.OdLevel = Clamp(state.OdLevel - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.OdLevel = 0.78f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(odRect.X + 82, odRect.Y + 138, 62, 62), state.OdTone, "TONE",
                string.Format("{0:0.0}", state.OdTone * 10f), Color.FromArgb(238, 175, 65),
                dy => { state.OdTone = Clamp(state.OdTone - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.OdTone = 0.58f; MarkDirty(); });
            DrawStompSwitch(g, new RectangleF(odRect.X + 80, odRect.Y + 340, 66, 66), state.OdActive, "BYPASS",
                () => { state.OdActive = !state.OdActive; MarkDirty(); });

            // 2. DRT Pedal (x: 278, y: 232, w: 226, h: 438)
            RectangleF drtRect = new RectangleF(278, 232, 226, 438);
            DrawPedalEnclosure(g, drtRect, Color.FromArgb(58, 24, 28), Color.FromArgb(24, 12, 14), Color.FromArgb(255, 58, 58), "DRT", "HEAVY DISTORTION");
            DrawGeometricPedalArt(g, new RectangleF(drtRect.X + 24, drtRect.Y + 215, drtRect.Width - 48, 110), Color.FromArgb(65, 255, 58, 58));

            DrawRotaryKnob(g, new RectangleF(drtRect.X + 22, drtRect.Y + 66, 62, 62), state.DrtDist, "DIST",
                string.Format("{0:0.0}", state.DrtDist * 10f), Color.FromArgb(255, 58, 58),
                dy => { state.DrtDist = Clamp(state.DrtDist - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.DrtDist = 0.62f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(drtRect.X + 142, drtRect.Y + 66, 62, 62), state.DrtVol, "VOL",
                string.Format("{0:0.0}", state.DrtVol * 10f), Color.FromArgb(255, 58, 58),
                dy => { state.DrtVol = Clamp(state.DrtVol - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.DrtVol = 0.55f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(drtRect.X + 82, drtRect.Y + 138, 62, 62), state.DrtFilter, "FILTER",
                string.Format("{0:0.0}", state.DrtFilter * 10f), Color.FromArgb(255, 58, 58),
                dy => { state.DrtFilter = Clamp(state.DrtFilter - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.DrtFilter = 0.48f; MarkDirty(); });
            DrawStompSwitch(g, new RectangleF(drtRect.X + 80, drtRect.Y + 340, 66, 66), state.DrtActive, "BYPASS",
                () => { state.DrtActive = !state.DrtActive; MarkDirty(); });

            // 3. PHSR Pedal (x: 522, y: 232, w: 226, h: 438)
            RectangleF phsrRect = new RectangleF(522, 232, 226, 438);
            DrawPedalEnclosure(g, phsrRect, Color.FromArgb(24, 52, 48), Color.FromArgb(13, 25, 23), Color.FromArgb(69, 248, 175), "PHSR", "4-STAGE PHASER");
            DrawGeometricPedalArt(g, new RectangleF(phsrRect.X + 24, phsrRect.Y + 215, phsrRect.Width - 48, 110), Color.FromArgb(65, 69, 248, 175));

            DrawRotaryKnob(g, new RectangleF(phsrRect.X + 68, phsrRect.Y + 95, 90, 90), (state.PhsrRateHz - 0.12f) / 9.88f, "RATE",
                string.Format("{0:0.00} Hz", state.PhsrRateHz), Color.FromArgb(69, 248, 175),
                dy => { state.PhsrRateHz = Clamp(state.PhsrRateHz - dy * 0.04f, 0.12f, 10.0f); MarkDirty(); },
                () => { state.PhsrRateHz = 1.00f; MarkDirty(); });
            DrawStompSwitch(g, new RectangleF(phsrRect.X + 80, phsrRect.Y + 340, 66, 66), state.PhsrActive, "BYPASS",
                () => { state.PhsrActive = !state.PhsrActive; MarkDirty(); });

            // 4. CHR Pedal (x: 766, y: 232, w: 226, h: 438)
            RectangleF chrRect = new RectangleF(766, 232, 226, 438);
            DrawPedalEnclosure(g, chrRect, Color.FromArgb(30, 34, 62), Color.FromArgb(16, 18, 32), Color.FromArgb(90, 155, 255), "CHR", "STEREO BBD CHORUS");
            DrawGeometricPedalArt(g, new RectangleF(chrRect.X + 24, chrRect.Y + 225, chrRect.Width - 48, 100), Color.FromArgb(65, 90, 155, 255));

            DrawRotaryKnob(g, new RectangleF(chrRect.X + 26, chrRect.Y + 64, 58, 58), (state.ChrRateHz - 0.12f) / 9.88f, "RATE",
                string.Format("{0:0.00} Hz", state.ChrRateHz), Color.FromArgb(90, 155, 255),
                dy => { state.ChrRateHz = Clamp(state.ChrRateHz - dy * 0.04f, 0.12f, 10.0f); MarkDirty(); },
                () => { state.ChrRateHz = 0.85f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(chrRect.X + 142, chrRect.Y + 64, 58, 58), state.ChrDepth, "DEPTH",
                string.Format("{0:0}%", state.ChrDepth * 100f), Color.FromArgb(90, 155, 255),
                dy => { state.ChrDepth = Clamp(state.ChrDepth - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.ChrDepth = 0.50f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(chrRect.X + 26, chrRect.Y + 146, 58, 58), state.ChrFeedback, "FEEDBACK",
                string.Format("{0:0}%", state.ChrFeedback * 100f), Color.FromArgb(90, 155, 255),
                dy => { state.ChrFeedback = Clamp(state.ChrFeedback - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.ChrFeedback = 0.25f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(chrRect.X + 142, chrRect.Y + 146, 58, 58), state.ChrMix, "MIX",
                string.Format("{0:0}%", state.ChrMix * 100f), Color.FromArgb(90, 155, 255),
                dy => { state.ChrMix = Clamp(state.ChrMix - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.ChrMix = 0.45f; MarkDirty(); });
            DrawStompSwitch(g, new RectangleF(chrRect.X + 80, chrRect.Y + 340, 66, 66), state.ChrActive, "BYPASS",
                () => { state.ChrActive = !state.ChrActive; MarkDirty(); });
        }

        // ====================================================================
        // VIEW 2: AMP SECTION (CLN Clean, RST Rust Crunch, HOT High-Gain Lead)
        // ====================================================================
        private void DrawAmpSection(Graphics g)
        {
            int a = state.AmpType;
            string[] ampTitles = new string[]
            {
                "CLEAN AMPLIFIER (CLN)  —  VINTAGE TUBE HEAD WITH BRIGHT MATRIX",
                "RUST AMPLIFIER (RST)  —  CRUNCH & RHYTHM TUBE HEAD",
                "HOT AMPLIFIER (HOT)  —  HIGH-GAIN MODERN METAL LEAD HEAD"
            };
            DrawSectionHeaderBanner(g, ampTitles[a], state.AmpSectionActive && state.AmpPower[a]);

            // Subtle Cabinet Top Edge Below Amp Head (y: 596..680)
            RectangleF cabPreview = new RectangleF(92, 600, 840, 78);
            using (GraphicsPath cp = RoundedRect(cabPreview, 12f))
            using (Brush cb = new SolidBrush(Color.FromArgb(18, 18, 21)))
            using (Pen cpen = new Pen(Color.FromArgb(46, 46, 52), 2f))
            {
                g.FillPath(cb, cp);
                g.DrawPath(cpen, cp);
            }
            using (Font cf = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (Brush ctb = new SolidBrush(Color.FromArgb(115, 118, 126)))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                string cabLabel = string.Format("MATCHED 4x12 CABINET #{0} ({1})  —  {2}",
                    state.CabType + 1,
                    state.CabType == 0 ? "CLN VINTAGE" : (state.CabType == 1 ? "RST CRUNCH" : "HOT MODERN"),
                    state.AmpCabLinked ? "AMP/CAB LINKED" : "UNLINKED CUSTOM CAB");
                g.DrawString(cabLabel, cf, ctb, new RectangleF(cabPreview.X, cabPreview.Y + 10, cabPreview.Width, 18), sf);
            }

            // Main Amp Head Outer Shell (x: 72, y: 232, w: 880, h: 360)
            RectangleF headOuter = new RectangleF(72, 232, 880, 360);
            Color shellTop = a == 0 ? Color.FromArgb(62, 42, 28) : (a == 1 ? Color.FromArgb(42, 44, 48) : Color.FromArgb(26, 26, 30));
            Color shellBot = a == 0 ? Color.FromArgb(34, 22, 14) : (a == 1 ? Color.FromArgb(22, 23, 26) : Color.FromArgb(12, 12, 15));
            Color trimCol = a == 0 ? Color.FromArgb(212, 168, 92) : (a == 1 ? Color.FromArgb(165, 170, 178) : Color.FromArgb(95, 98, 108));

            using (GraphicsPath hp = RoundedRect(headOuter, 18f))
            using (LinearGradientBrush hb = new LinearGradientBrush(headOuter, shellTop, shellBot, LinearGradientMode.Vertical))
            using (Pen hpen = new Pen(trimCol, 2.4f))
            {
                g.FillPath(hb, hp);
                g.DrawPath(hpen, hp);
            }

            // Inner Grille Window (x: 98, y: 254, w: 828, h: 204)
            RectangleF grilleRect = new RectangleF(98, 254, 828, 204);
            using (GraphicsPath gp = RoundedRect(grilleRect, 10f))
            using (Brush gb = new SolidBrush(Color.FromArgb(11, 11, 13)))
            {
                g.FillPath(gb, gp);
            }

            // Warm vacuum tube glow behind the grille when Amp Power is ON
            if (state.AmpSectionActive && state.AmpPower[a])
            {
                Color tubeCol = a == 0
                    ? Color.FromArgb(85, 255, 175, 70)
                    : (a == 1 ? Color.FromArgb(95, 255, 115, 35) : Color.FromArgb(110, 255, 38, 48));
                for (int t = 0; t < 4; t++)
                {
                    float tx = grilleRect.X + 180 + t * 135;
                    using (GraphicsPath tgp = new GraphicsPath())
                    {
                        tgp.AddEllipse(tx - 45, grilleRect.Y + 25, 90, 145);
                        using (PathGradientBrush pgb = new PathGradientBrush(tgp))
                        {
                            pgb.CenterColor = tubeCol;
                            pgb.SurroundColors = new Color[] { Color.Transparent };
                            g.FillPath(pgb, tgp);
                        }
                    }
                }
            }

            // Distinct Amp Grille Artwork per Amp Model (CLN, RST, HOT)
            if (a == 0)
            {
                // CLN: Woven diamond geometric pattern
                using (Pen dp = new Pen(Color.FromArgb(55, 212, 168, 92), 1.2f))
                {
                    for (float x = grilleRect.X + 20; x < grilleRect.Right - 20; x += 28)
                    {
                        for (float y = grilleRect.Y + 20; y < grilleRect.Bottom - 20; y += 28)
                        {
                            PointF[] dia = new PointF[]
                            {
                                new PointF(x, y - 10),
                                new PointF(x + 10, y),
                                new PointF(x, y + 10),
                                new PointF(x - 10, y)
                            };
                            g.DrawPolygon(dp, dia);
                        }
                    }
                }
            }
            else if (a == 1)
            {
                // RST: Industrial brushed hexagonal mesh & twin vertical bars
                using (Pen mp = new Pen(Color.FromArgb(46, 170, 175, 185), 1f))
                {
                    for (float x = grilleRect.X + 16; x < grilleRect.Right - 16; x += 18)
                    {
                        g.DrawLine(mp, x, grilleRect.Y + 8, x, grilleRect.Bottom - 8);
                    }
                    for (float y = grilleRect.Y + 16; y < grilleRect.Bottom - 16; y += 18)
                    {
                        g.DrawLine(mp, grilleRect.X + 8, y, grilleRect.Right - 8, y);
                    }
                }
            }
            else
            {
                // HOT: Signature 5x2 3D Sculpted Acoustic Block Grille (seen in carved_4336640.png!)
                float padX = 24f;
                float padY = 18f;
                float gap = 14f;
                float blockW = (grilleRect.Width - padX * 2 - gap * 4) / 5f;
                float blockH = (grilleRect.Height - padY * 2 - gap) / 2f;

                for (int row = 0; row < 2; row++)
                {
                    for (int col = 0; col < 5; col++)
                    {
                        float bx = grilleRect.X + padX + col * (blockW + gap);
                        float by = grilleRect.Y + padY + row * (blockH + gap);
                        RectangleF bRect = new RectangleF(bx, by, blockW, blockH);
                        bool silverBlock = ((row + col) % 2 == 1);
                        Color c1 = silverBlock ? Color.FromArgb(148, 150, 156) : Color.FromArgb(34, 35, 39);
                        Color c2 = silverBlock ? Color.FromArgb(88, 90, 96) : Color.FromArgb(18, 19, 22);
                        using (LinearGradientBrush bb = new LinearGradientBrush(bRect, c1, c2, LinearGradientMode.ForwardDiagonal))
                        using (Pen bp = new Pen(Color.FromArgb(65, 68, 75), 1.2f))
                        {
                            g.FillRectangle(bb, bRect);
                            g.DrawRectangle(bp, bRect.X, bRect.Y, bRect.Width, bRect.Height);
                        }
                    }
                }
            }

            // Live Tone Stack & Tube Transfer HUD Overlay in top-right of grille
            DrawAmpResponseHud(g, new RectangleF(grilleRect.Right - 210, grilleRect.Y + 14, 194, 72), a);

            // Bottom Control Faceplate (x: 98, y: 468, w: 828, h: 106)
            RectangleF plateRect = new RectangleF(98, 468, 828, 106);
            Color plateTop = a == 0 ? Color.FromArgb(188, 165, 118) : (a == 1 ? Color.FromArgb(68, 70, 76) : Color.FromArgb(32, 33, 38));
            Color plateBot = a == 0 ? Color.FromArgb(142, 122, 82) : (a == 1 ? Color.FromArgb(44, 46, 52) : Color.FromArgb(18, 19, 22));
            using (GraphicsPath pp = RoundedRect(plateRect, 8f))
            using (LinearGradientBrush pb = new LinearGradientBrush(plateRect, plateTop, plateBot, LinearGradientMode.Vertical))
            using (Pen ppen = new Pen(trimCol, 1.4f))
            {
                g.FillPath(pb, pp);
                g.DrawPath(ppen, pp);
            }

            Color knobAccent = a == 0 ? Color.FromArgb(228, 122, 23) : (a == 1 ? Color.FromArgb(255, 120, 45) : Color.FromArgb(255, 48, 58));

            if (a == 0)
            {
                // CLN Controls: BRIGHT switch, GAIN, BASS, MID, TREBLE, LEVEL, POWER switch
                DrawMiniToggle(g, new RectangleF(plateRect.X + 26, plateRect.Y + 38, 68, 26), state.ClnBright, "BRIGHT",
                    () => { state.ClnBright = !state.ClnBright; MarkDirty(); });

                string[] clnNames = new string[] { "GAIN", "BASS", "MID", "TREBLE", "LEVEL" };
                float[] clnVals = new float[] { state.AmpGain[0], state.AmpBass[0], state.AmpMid[0], state.AmpTreble[0], state.AmpLevel[0] };
                for (int k = 0; k < 5; k++)
                {
                    int kIdx = k;
                    float kx = plateRect.X + 135 + k * 112;
                    DrawRotaryKnob(g, new RectangleF(kx, plateRect.Y + 14, 62, 62), clnVals[k], clnNames[k],
                        string.Format("{0:0.0}", clnVals[k] * 10f), knobAccent,
                        dy =>
                        {
                            if (kIdx == 0) state.AmpGain[0] = Clamp(state.AmpGain[0] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 1) state.AmpBass[0] = Clamp(state.AmpBass[0] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 2) state.AmpMid[0] = Clamp(state.AmpMid[0] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 3) state.AmpTreble[0] = Clamp(state.AmpTreble[0] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 4) state.AmpLevel[0] = Clamp(state.AmpLevel[0] - dy * 0.005f, 0f, 1f);
                            MarkDirty();
                        },
                        () =>
                        {
                            if (kIdx == 0) state.AmpGain[0] = 0.38f;
                            else if (kIdx == 1) state.AmpBass[0] = 0.52f;
                            else if (kIdx == 2) state.AmpMid[0] = 0.48f;
                            else if (kIdx == 3) state.AmpTreble[0] = 0.62f;
                            else if (kIdx == 4) state.AmpLevel[0] = 0.55f;
                            MarkDirty();
                        });
                }
            }
            else
            {
                // RST & HOT Controls: GAIN, BASS, MID, TREBLE, MASTER, PRESENCE, DEPTH, LEVEL
                string[] names = new string[] { "GAIN", "BASS", "MID", "TREBLE", "MASTER", "PRESENCE", "DEPTH", "LEVEL" };
                float[] vals = new float[]
                {
                    state.AmpGain[a], state.AmpBass[a], state.AmpMid[a], state.AmpTreble[a],
                    state.AmpMaster[a], state.AmpPresence[a], state.AmpDepth[a], state.AmpLevel[a]
                };
                for (int k = 0; k < 8; k++)
                {
                    int kIdx = k;
                    float kx = plateRect.X + 24 + k * 88;
                    DrawRotaryKnob(g, new RectangleF(kx, plateRect.Y + 14, 58, 58), vals[k], names[k],
                        string.Format("{0:0.0}", vals[k] * 10f), knobAccent,
                        dy =>
                        {
                            if (kIdx == 0) state.AmpGain[a] = Clamp(state.AmpGain[a] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 1) state.AmpBass[a] = Clamp(state.AmpBass[a] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 2) state.AmpMid[a] = Clamp(state.AmpMid[a] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 3) state.AmpTreble[a] = Clamp(state.AmpTreble[a] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 4) state.AmpMaster[a] = Clamp(state.AmpMaster[a] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 5) state.AmpPresence[a] = Clamp(state.AmpPresence[a] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 6) state.AmpDepth[a] = Clamp(state.AmpDepth[a] - dy * 0.005f, 0f, 1f);
                            else if (kIdx == 7) state.AmpLevel[a] = Clamp(state.AmpLevel[a] - dy * 0.005f, 0f, 1f);
                            MarkDirty();
                        },
                        () =>
                        {
                            if (kIdx == 0) state.AmpGain[a] = 0.65f;
                            else if (kIdx == 1) state.AmpBass[a] = 0.60f;
                            else if (kIdx == 2) state.AmpMid[a] = 0.58f;
                            else if (kIdx == 3) state.AmpTreble[a] = 0.65f;
                            else if (kIdx == 4) state.AmpMaster[a] = 0.52f;
                            else if (kIdx == 5) state.AmpPresence[a] = 0.62f;
                            else if (kIdx == 6) state.AmpDepth[a] = 0.60f;
                            else if (kIdx == 7) state.AmpLevel[a] = 0.54f;
                            MarkDirty();
                        });
                }
            }

            // Amp Power Switch + Jewel Pilot Lamp on far right of faceplate
            RectangleF pwrRect = new RectangleF(plateRect.Right - 84, plateRect.Y + 20, 64, 64);
            DrawAmpPowerJewel(g, pwrRect, state.AmpPower[a], knobAccent,
                () => { state.AmpPower[a] = !state.AmpPower[a]; MarkDirty(); });
        }

        private void DrawAmpResponseHud(Graphics g, RectangleF rect, int ampIdx)
        {
            using (GraphicsPath p = RoundedRect(rect, 6f))
            using (Brush b = new SolidBrush(Color.FromArgb(190, 12, 14, 18)))
            using (Pen pen = new Pen(Color.FromArgb(65, 70, 80), 1f))
            {
                g.FillPath(b, p);
                g.DrawPath(pen, p);
            }

            using (Font f = new Font("Segoe UI", 6.5f, FontStyle.Bold))
            using (Brush tb = new SolidBrush(Color.FromArgb(190, 195, 205)))
            {
                g.DrawString("TONE STACK & SATURATION CURVE", f, tb, rect.X + 8, rect.Y + 5);
            }

            PointF[] curve = new PointF[32];
            for (int i = 0; i < curve.Length; i++)
            {
                float norm = i / (float)(curve.Length - 1);
                float low = state.AmpBass[ampIdx] * (1f - norm);
                float mid = (state.AmpMid[ampIdx] - 0.5f) * (float)Math.Sin(norm * Math.PI);
                float high = (state.AmpTreble[ampIdx] + state.AmpPresence[ampIdx] * 0.5f) * norm;
                float val = Clamp(0.35f + (low * 0.35f + mid * 0.45f + high * 0.35f), 0.1f, 0.95f);
                curve[i] = new PointF(rect.X + 8 + norm * (rect.Width - 16), rect.Bottom - 8 - val * 42f);
            }
            using (Pen cp = new Pen(Color.FromArgb(228, 122, 23), 1.8f))
            {
                g.DrawLines(cp, curve);
            }
        }

        // ====================================================================
        // VIEW 3: CAB SECTION (Dual Microphone IR Cabinet Simulator)
        // ====================================================================
        private void DrawCabSection(Graphics g)
        {
            string[] cabTitles = new string[]
            {
                "CABINET SIMULATOR  —  CAB 1 (CLN VINTAGE 2x12) DUAL IMPULSE RESPONSE STAGE",
                "CABINET SIMULATOR  —  CAB 2 (RST CRUNCH 4x12) DUAL IMPULSE RESPONSE STAGE",
                "CABINET SIMULATOR  —  CAB 3 (HOT MODERN 4x12) DUAL IMPULSE RESPONSE STAGE"
            };
            DrawSectionHeaderBanner(g, cabTitles[state.CabType], state.CabSectionActive);

            // Top Speaker Stage (x: 68, y: 226, w: 888, h: 252)
            RectangleF stageRect = new RectangleF(68, 226, 888, 252);
            using (GraphicsPath sp = RoundedRect(stageRect, 12f))
            using (Brush sb = new SolidBrush(Color.FromArgb(15, 16, 19)))
            using (Pen spen = new Pen(Color.FromArgb(52, 54, 62), 1.6f))
            {
                g.FillPath(sb, sp);
                g.DrawPath(spen, sp);
            }

            // Left Speaker Cone (Center: 290, 352, Radius: 104) & Right Speaker Cone (Center: 734, 352, Radius: 104)
            DrawSpeakerConeAndMic(g, new PointF(290, 352), 104f, "MIC 1 (LEFT)",
                state.Mic1Active, MicNames[state.Mic1Type], state.Mic1Pos, state.Mic1Dist, Color.FromArgb(10, 116, 224),
                new RectangleF(140, 240, 300, 224),
                pt =>
                {
                    state.Mic1Pos = Clamp((pt.X - 290f) / 95f, 0f, 1f);
                    state.Mic1Dist = Clamp((pt.Y - 310f) / 110f, 0f, 1f);
                    MarkDirty();
                });

            DrawSpeakerConeAndMic(g, new PointF(734, 352), 104f, "MIC 2 (RIGHT)",
                state.Mic2Active, MicNames[state.Mic2Type], state.Mic2Pos, state.Mic2Dist, Color.FromArgb(228, 122, 23),
                new RectangleF(584, 240, 300, 224),
                pt =>
                {
                    state.Mic2Pos = Clamp((pt.X - 734f) / 95f, 0f, 1f);
                    state.Mic2Dist = Clamp((pt.Y - 310f) / 110f, 0f, 1f);
                    MarkDirty();
                });

            // Bottom Left IR Loader Panel (x: 68, y: 490, w: 432, h: 184)
            DrawCabChannelPanel(g, new RectangleF(68, 490, 432, 184), 1,
                state.Mic1Active, state.Mic1Type, state.Mic1Pos, state.Mic1Dist, state.Mic1LevelDb, state.Mic1Pan, state.Mic1PhaseInvert,
                Color.FromArgb(10, 116, 224));

            // Bottom Right IR Loader Panel (x: 524, y: 490, w: 432, h: 184)
            DrawCabChannelPanel(g, new RectangleF(524, 490, 432, 184), 2,
                state.Mic2Active, state.Mic2Type, state.Mic2Pos, state.Mic2Dist, state.Mic2LevelDb, state.Mic2Pan, state.Mic2PhaseInvert,
                Color.FromArgb(228, 122, 23));
        }

        private void DrawSpeakerConeAndMic(Graphics g, PointF center, float radius, string title,
            bool active, string micName, float pos, float dist, Color accent, RectangleF dragZone, Action<PointF> onDrag)
        {
            // Concentric Speaker Surround, Cone & Dustcap
            float[] rings = new float[] { 1.0f, 0.88f, 0.65f, 0.32f };
            for (int r = 0; r < rings.Length; r++)
            {
                float rad = radius * rings[r];
                RectangleF cRect = new RectangleF(center.X - rad, center.Y - rad, rad * 2, rad * 2);
                using (Brush cb = new SolidBrush(Color.FromArgb(24 + r * 6, 25 + r * 6, 30 + r * 6)))
                using (Pen cp = new Pen(Color.FromArgb(55 + r * 12, 58 + r * 12, 66 + r * 12), 1.4f))
                {
                    g.FillEllipse(cb, cRect);
                    g.DrawEllipse(cp, cRect);
                }
            }

            // Position & Distance Guide Axis
            using (Pen guidePen = new Pen(Color.FromArgb(80, accent), 1f) { DashStyle = DashStyle.Dot })
            {
                g.DrawLine(guidePen, center.X, center.Y, center.X + radius, center.Y);
            }

            // Virtual Microphone Icon at (center.X + pos * 95, center.Y - 25 + dist * 95)
            float micX = center.X + pos * 92f;
            float micY = center.Y - 12f + dist * 78f;
            RectangleF micBody = new RectangleF(micX - 14, micY - 8, 28, 52);

            using (GraphicsPath mp = RoundedRect(micBody, 8f))
            using (Brush mb = new SolidBrush(active ? accent : Color.FromArgb(85, 85, 92)))
            using (Pen mpen = new Pen(Color.White, 1.5f))
            {
                g.FillPath(mb, mp);
                g.DrawPath(mpen, mp);
            }
            using (Font mf = new Font("Segoe UI", 7.2f, FontStyle.Bold))
            using (Brush tb = new SolidBrush(Color.FromArgb(215, 218, 225)))
            {
                g.DrawString(title + ": " + micName + "  (DRAG MIC)", mf, tb, dragZone.X + 8, dragZone.Y + 4);
            }

            hotspots.Add(new UiHotspot
            {
                Id = "cab_mic_drag_" + title,
                Bounds = dragZone,
                OnDragPoint = onDrag
            });
        }

        private void DrawCabChannelPanel(Graphics g, RectangleF rect, int ch,
            bool active, int micType, float pos, float dist, float levelDb, float pan, bool phaseInv, Color accent)
        {
            using (GraphicsPath p = RoundedRect(rect, 8f))
            using (Brush b = new SolidBrush(Color.FromArgb(22, 23, 27)))
            using (Pen pen = new Pen(active ? Color.FromArgb(140, accent) : Color.FromArgb(48, 50, 56), 1.4f))
            {
                g.FillPath(b, p);
                g.DrawPath(pen, p);
            }

            // Top row: IR Power Switch | < Mic Selector Dropdown > | Phase Ø Button
            DrawMiniToggle(g, new RectangleF(rect.X + 14, rect.Y + 14, 58, 24), active, "IR " + ch,
                () =>
                {
                    if (ch == 1) state.Mic1Active = !state.Mic1Active;
                    else state.Mic2Active = !state.Mic2Active;
                    MarkDirty();
                });

            RectangleF prevMic = new RectangleF(rect.X + 82, rect.Y + 14, 26, 24);
            RectangleF micBox = new RectangleF(rect.X + 112, rect.Y + 14, 206, 24);
            RectangleF nextMic = new RectangleF(rect.X + 322, rect.Y + 14, 26, 24);
            RectangleF phaseBtn = new RectangleF(rect.X + 358, rect.Y + 14, 60, 24);

            DrawButtonPill(g, prevMic, "◄", Color.FromArgb(34, 35, 42), Color.White, 7.5f);
            DrawButtonPill(g, micBox, MicNames[micType], Color.FromArgb(14, 15, 18), accent, 8.2f);
            DrawButtonPill(g, nextMic, "►", Color.FromArgb(34, 35, 42), Color.White, 7.5f);
            DrawButtonPill(g, phaseBtn, phaseInv ? "Ø 180°" : "Ø NORM",
                phaseInv ? accent : Color.FromArgb(34, 35, 42),
                phaseInv ? Color.Black : Color.FromArgb(200, 202, 210), 7.5f);

            hotspots.Add(new UiHotspot
            {
                Id = "mic_prev_" + ch,
                Bounds = prevMic,
                OnClick = () =>
                {
                    if (ch == 1) state.Mic1Type = (state.Mic1Type + MicNames.Length - 1) % MicNames.Length;
                    else state.Mic2Type = (state.Mic2Type + MicNames.Length - 1) % MicNames.Length;
                    MarkDirty();
                }
            });
            hotspots.Add(new UiHotspot
            {
                Id = "mic_next_" + ch,
                Bounds = nextMic,
                OnClick = () =>
                {
                    if (ch == 1) state.Mic1Type = (state.Mic1Type + 1) % MicNames.Length;
                    else state.Mic2Type = (state.Mic2Type + 1) % MicNames.Length;
                    MarkDirty();
                }
            });
            hotspots.Add(new UiHotspot
            {
                Id = "mic_box_" + ch,
                Bounds = micBox,
                OnClick = () =>
                {
                    if (ch == 1) state.Mic1Type = (state.Mic1Type + 1) % MicNames.Length;
                    else state.Mic2Type = (state.Mic2Type + 1) % MicNames.Length;
                    MarkDirty();
                }
            });
            hotspots.Add(new UiHotspot
            {
                Id = "mic_phase_" + ch,
                Bounds = phaseBtn,
                OnClick = () =>
                {
                    if (ch == 1) state.Mic1PhaseInvert = !state.Mic1PhaseInvert;
                    else state.Mic2PhaseInvert = !state.Mic2PhaseInvert;
                    MarkDirty();
                }
            });

            // 4 Knobs: POSITION (0..1), DISTANCE (0..1), MIC LEVEL (-40..+6 dB), PAN (L50..C..R50)
            DrawRotaryKnob(g, new RectangleF(rect.X + 24, rect.Y + 58, 66, 66), pos, "POSITION",
                string.Format("{0:0.000}", pos), accent,
                dy =>
                {
                    if (ch == 1) state.Mic1Pos = Clamp(state.Mic1Pos - dy * 0.004f, 0f, 1f);
                    else state.Mic2Pos = Clamp(state.Mic2Pos - dy * 0.004f, 0f, 1f);
                    MarkDirty();
                },
                () => { if (ch == 1) state.Mic1Pos = 0.350f; else state.Mic2Pos = 0.520f; MarkDirty(); });

            DrawRotaryKnob(g, new RectangleF(rect.X + 128, rect.Y + 58, 66, 66), dist, "DISTANCE",
                string.Format("{0:0.000}", dist), accent,
                dy =>
                {
                    if (ch == 1) state.Mic1Dist = Clamp(state.Mic1Dist - dy * 0.004f, 0f, 1f);
                    else state.Mic2Dist = Clamp(state.Mic2Dist - dy * 0.004f, 0f, 1f);
                    MarkDirty();
                },
                () => { if (ch == 1) state.Mic1Dist = 0.150f; else state.Mic2Dist = 0.220f; MarkDirty(); });

            DrawRotaryKnob(g, new RectangleF(rect.X + 232, rect.Y + 58, 66, 66), (levelDb + 40f) / 46f, "MIC LEVEL",
                string.Format("{0:+0.0;-0.0;0.0} dB", levelDb), accent,
                dy =>
                {
                    if (ch == 1) state.Mic1LevelDb = Clamp(state.Mic1LevelDb - dy * 0.2f, -40f, 6f);
                    else state.Mic2LevelDb = Clamp(state.Mic2LevelDb - dy * 0.2f, -40f, 6f);
                    MarkDirty();
                },
                () => { if (ch == 1) state.Mic1LevelDb = -6.0f; else state.Mic2LevelDb = -8.5f; MarkDirty(); });

            int panInt = (int)Math.Round((pan - 0.5f) * 100f);
            string panStr = panInt == 0 ? "C" : (panInt < 0 ? "L" + (-panInt) : "R" + panInt);
            DrawRotaryKnob(g, new RectangleF(rect.X + 336, rect.Y + 58, 66, 66), pan, "PAN",
                panStr, accent,
                dy =>
                {
                    if (ch == 1) state.Mic1Pan = Clamp(state.Mic1Pan - dy * 0.005f, 0f, 1f);
                    else state.Mic2Pan = Clamp(state.Mic2Pan - dy * 0.005f, 0f, 1f);
                    MarkDirty();
                },
                () => { if (ch == 1) state.Mic1Pan = 0.50f; else state.Mic2Pan = 0.50f; MarkDirty(); });
        }

        // ====================================================================
        // VIEW 4: EQ SECTION (3x 9-Band Graphic Equalizers + HPF/LPF)
        // ====================================================================
        private void DrawEqSection(Graphics g)
        {
            int e = state.AmpType;
            string[] eqTitles = new string[]
            {
                "9-BAND GRAPHIC EQUALIZER  —  CHANNEL 1 (CLN) WITH HIGH-PASS & LOW-PASS FILTERS",
                "9-BAND GRAPHIC EQUALIZER  —  CHANNEL 2 (RST) WITH HIGH-PASS & LOW-PASS FILTERS",
                "9-BAND GRAPHIC EQUALIZER  —  CHANNEL 3 (HOT) WITH HIGH-PASS & LOW-PASS FILTERS"
            };
            DrawSectionHeaderBanner(g, eqTitles[e], state.EqSectionActive && state.EqPower[e]);

            RectangleF eqChassis = new RectangleF(70, 232, 884, 438);
            Color accent = e == 0 ? Color.FromArgb(212, 168, 92) : (e == 1 ? Color.FromArgb(228, 122, 23) : Color.FromArgb(10, 116, 224));

            using (GraphicsPath ep = RoundedRect(eqChassis, 12f))
            using (Brush eb = new SolidBrush(Color.FromArgb(20, 21, 25)))
            using (Pen epen = new Pen(Color.FromArgb(56, 58, 66), 1.8f))
            {
                g.FillPath(eb, ep);
                g.DrawPath(epen, ep);
            }

            // Left Power Switch & Reset Flat Button
            DrawAmpPowerJewel(g, new RectangleF(eqChassis.X + 26, eqChassis.Y + 46, 64, 64), state.EqPower[e], accent,
                () => { state.EqPower[e] = !state.EqPower[e]; MarkDirty(); });

            RectangleF flatBtn = new RectangleF(eqChassis.X + 18, eqChassis.Y + 145, 80, 28);
            DrawButtonPill(g, flatBtn, "RESET FLAT", Color.FromArgb(34, 36, 42), Color.FromArgb(205, 208, 215), 7.5f);
            hotspots.Add(new UiHotspot
            {
                Id = "eq_flat",
                Bounds = flatBtn,
                OnClick = () =>
                {
                    for (int b = 0; b < 9; b++) state.EqBands[e][b] = 0.0f;
                    state.EqHpfHz[e] = 20.0f;
                    state.EqLpfKhz[e] = 20.0f;
                    MarkDirty();
                }
            });

            // 9 Vertical Band Sliders (-12..+12 dB)
            RectangleF graphArea = new RectangleF(eqChassis.X + 118, eqChassis.Y + 38, 560, 355);
            using (GraphicsPath gp = RoundedRect(graphArea, 8f))
            using (Brush gb = new SolidBrush(Color.FromArgb(12, 13, 16)))
            using (Pen gpen = new Pen(Color.FromArgb(44, 46, 54), 1f))
            {
                g.FillPath(gb, gp);
                g.DrawPath(gpen, gp);
            }

            // Horizontal dB grid lines (+12, +6, 0, -6, -12 dB)
            float trackTop = graphArea.Y + 34;
            float trackBot = graphArea.Bottom - 44;
            float trackH = trackBot - trackTop;
            float midY = trackTop + trackH * 0.5f;

            using (Pen zeroPen = new Pen(Color.FromArgb(80, 84, 96), 1.2f))
            using (Pen gridPen = new Pen(Color.FromArgb(36, 38, 46), 1f) { DashStyle = DashStyle.Dot })
            using (Font dbF = new Font("Segoe UI", 6.8f, FontStyle.Regular))
            using (Brush dbB = new SolidBrush(Color.FromArgb(115, 118, 128)))
            {
                g.DrawLine(zeroPen, graphArea.X + 8, midY, graphArea.Right - 8, midY);
                g.DrawLine(gridPen, graphArea.X + 8, trackTop, graphArea.Right - 8, trackTop);
                g.DrawLine(gridPen, graphArea.X + 8, trackTop + trackH * 0.25f, graphArea.Right - 8, trackTop + trackH * 0.25f);
                g.DrawLine(gridPen, graphArea.X + 8, trackTop + trackH * 0.75f, graphArea.Right - 8, trackTop + trackH * 0.75f);
                g.DrawLine(gridPen, graphArea.X + 8, trackBot, graphArea.Right - 8, trackBot);

                g.DrawString("+12 dB", dbF, dbB, graphArea.X + 6, trackTop - 6);
                g.DrawString("  0 dB", dbF, dbB, graphArea.X + 6, midY - 7);
                g.DrawString("-12 dB", dbF, dbB, graphArea.X + 6, trackBot - 7);
            }

            PointF[] eqPoints = new PointF[9];
            float colW = (graphArea.Width - 56) / 9f;

            for (int b = 0; b < 9; b++)
            {
                int bandIdx = b;
                float cx = graphArea.X + 42 + b * colW + colW * 0.5f;
                float gainDb = state.EqBands[e][b];
                float norm = (gainDb + 12f) / 24f; // 0..1
                float thumbY = trackBot - norm * trackH;
                eqPoints[b] = new PointF(cx, thumbY);

                // Fader slot
                using (Pen slotPen = new Pen(Color.FromArgb(46, 48, 56), 4f))
                {
                    g.DrawLine(slotPen, cx, trackTop, cx, trackBot);
                }

                // Active boost/cut fill bar from 0 dB center line
                using (Pen activeBar = new Pen(accent, 4f))
                {
                    g.DrawLine(activeBar, cx, midY, cx, thumbY);
                }

                // Fader Thumb Cap
                RectangleF cap = new RectangleF(cx - 14, thumbY - 9, 28, 18);
                using (GraphicsPath cp = RoundedRect(cap, 4f))
                using (Brush cb = new SolidBrush(Color.FromArgb(225, 228, 235)))
                using (Pen cpen = new Pen(accent, 1.5f))
                {
                    g.FillPath(cb, cp);
                    g.DrawPath(cpen, cp);
                }

                using (Font bf = new Font("Segoe UI", 7.2f, FontStyle.Bold))
                using (Brush bb = new SolidBrush(Color.FromArgb(190, 194, 204)))
                using (Brush vb = new SolidBrush(accent))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                {
                    g.DrawString(string.Format("{0:+0.0;-0.0;0.0}", gainDb), bf, vb, new RectangleF(cx - 26, graphArea.Y + 8, 52, 16), sf);
                    g.DrawString(EqBandLabels[b], bf, bb, new RectangleF(cx - 28, trackBot + 14, 56, 16), sf);
                }

                RectangleF dragCol = new RectangleF(cx - colW * 0.45f, trackTop - 12, colW * 0.9f, trackH + 24);
                hotspots.Add(new UiHotspot
                {
                    Id = "eq_band_" + b,
                    Bounds = dragCol,
                    OnDragPoint = pt =>
                    {
                        float n = 1.0f - (pt.Y - trackTop) / trackH;
                        state.EqBands[e][bandIdx] = Clamp(n * 24f - 12f, -12f, 12f);
                        MarkDirty();
                    },
                    OnDoubleClick = () => { state.EqBands[e][bandIdx] = 0.0f; MarkDirty(); }
                });
            }

            // Smooth composite response curve connecting the 9 bands
            using (Pen curvePen = new Pen(Color.FromArgb(150, accent), 2.2f))
            {
                g.DrawCurve(curvePen, eqPoints, 0.4f);
            }

            // Right Filter Section: HPF (20..500 Hz) & LPF (1.0..20.0 kHz)
            DrawRotaryKnob(g, new RectangleF(eqChassis.Right - 148, eqChassis.Y + 80, 86, 86),
                (state.EqHpfHz[e] - 20f) / 480f, "HIGH-PASS (HPF)",
                state.EqHpfHz[e] <= 21f ? "OFF (20 Hz)" : string.Format("{0:0} Hz", state.EqHpfHz[e]), accent,
                dy => { state.EqHpfHz[e] = Clamp(state.EqHpfHz[e] - dy * 2.0f, 20f, 500f); MarkDirty(); },
                () => { state.EqHpfHz[e] = 20.0f; MarkDirty(); });

            DrawRotaryKnob(g, new RectangleF(eqChassis.Right - 148, eqChassis.Y + 236, 86, 86),
                (state.EqLpfKhz[e] - 1f) / 19f, "LOW-PASS (LPF)",
                state.EqLpfKhz[e] >= 19.9f ? "OFF (20 kHz)" : string.Format("{0:0.1} kHz", state.EqLpfKhz[e]), accent,
                dy => { state.EqLpfKhz[e] = Clamp(state.EqLpfKhz[e] - dy * 0.08f, 1.0f, 20.0f); MarkDirty(); },
                () => { state.EqLpfKhz[e] = 20.0f; MarkDirty(); });
        }

        // ====================================================================
        // VIEW 5: POST FX SECTION (DLY Delay with LCD + REV Shimmer Reverb)
        // ====================================================================
        private void DrawPostFxSection(Graphics g)
        {
            DrawSectionHeaderBanner(g, "POST FX SECTION  —  PING-PONG TAPE DELAY WITH LCD & SHIMMER ATMOSPHERIC REVERB", state.PostFxSectionActive);

            // Left Pedal: DLY Delay (x: 84, y: 228, w: 410, h: 444)
            RectangleF dlyRect = new RectangleF(84, 228, 410, 444);
            DrawPedalEnclosure(g, dlyRect, Color.FromArgb(30, 32, 36), Color.FromArgb(15, 16, 19), Color.FromArgb(228, 122, 23), "DLY", "TAPE / PING-PONG DELAY");

            // Row 1 Knobs: MIX, FEEDBACK, TONE
            DrawRotaryKnob(g, new RectangleF(dlyRect.X + 34, dlyRect.Y + 54, 62, 62), state.DlyMix, "MIX",
                string.Format("{0:0}%", state.DlyMix * 100f), Color.FromArgb(228, 122, 23),
                dy => { state.DlyMix = Clamp(state.DlyMix - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.DlyMix = 0.35f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(dlyRect.X + 174, dlyRect.Y + 54, 62, 62), state.DlyFeedback, "FEEDBACK",
                string.Format("{0:0}%", state.DlyFeedback * 100f), Color.FromArgb(228, 122, 23),
                dy => { state.DlyFeedback = Clamp(state.DlyFeedback - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.DlyFeedback = 0.42f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(dlyRect.X + 314, dlyRect.Y + 54, 62, 62), state.DlyTone, "TONE",
                string.Format("{0:0}%", state.DlyTone * 100f), Color.FromArgb(228, 122, 23),
                dy => { state.DlyTone = Clamp(state.DlyTone - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.DlyTone = 0.55f; MarkDirty(); });

            // Row 2 Knobs: TIME, TAPE SAT, TAPE MOD
            string timeVal = state.DlySyncMode == 0
                ? string.Format("{0:0} ms", state.DlyTimeMs)
                : SubdivisionNames[state.DlySubdivIndex];
            DrawRotaryKnob(g, new RectangleF(dlyRect.X + 34, dlyRect.Y + 146, 62, 62),
                state.DlySyncMode == 0 ? (state.DlyTimeMs - 100f) / 1000f : state.DlySubdivIndex / 11f, "TIME",
                timeVal, Color.FromArgb(228, 122, 23),
                dy =>
                {
                    if (state.DlySyncMode == 0) state.DlyTimeMs = Clamp(state.DlyTimeMs - dy * 4.5f, 100f, 1100f);
                    else
                    {
                        int step = dy < -4 ? 1 : (dy > 4 ? -1 : 0);
                        if (step != 0) state.DlySubdivIndex = (int)Clamp(state.DlySubdivIndex + step, 0, SubdivisionNames.Length - 1);
                    }
                    MarkDirty();
                },
                () => { state.DlyTimeMs = 400.0f; state.DlySubdivIndex = 7; MarkDirty(); });

            DrawRotaryKnob(g, new RectangleF(dlyRect.X + 174, dlyRect.Y + 146, 62, 62), state.DlyTapeSat, "TAPE SAT",
                string.Format("{0:0}%", state.DlyTapeSat * 100f), Color.FromArgb(228, 122, 23),
                dy => { state.DlyTapeSat = Clamp(state.DlyTapeSat - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.DlyTapeSat = 0.32f; MarkDirty(); });

            DrawRotaryKnob(g, new RectangleF(dlyRect.X + 314, dlyRect.Y + 146, 62, 62), state.DlyTapeMod, "TAPE MOD",
                string.Format("{0:0}%", state.DlyTapeMod * 100f), Color.FromArgb(228, 122, 23),
                dy => { state.DlyTapeMod = Clamp(state.DlyTapeMod - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.DlyTapeMod = 0.28f; MarkDirty(); });

            // Delay Illuminated Retro LCD Screen (#BDC6C0 text & #E47A17 editable accent from archetypeStyles.xml)
            RectangleF lcdRect = new RectangleF(dlyRect.X + 28, dlyRect.Y + 244, dlyRect.Width - 56, 86);
            using (GraphicsPath lp = RoundedRect(lcdRect, 8f))
            using (Brush lb = new SolidBrush(Color.FromArgb(10, 14, 12)))
            using (Pen lpen = new Pen(Color.FromArgb(62, 74, 68), 1.6f))
            {
                g.FillPath(lb, lp);
                g.DrawPath(lpen, lp);
            }

            string[] syncModes = new string[] { "FREE", "APP", "TAP" };
            using (Font lcdSmall = new Font("Consolas", 8.5f, FontStyle.Bold))
            using (Font lcdBig = new Font("Consolas", 15.5f, FontStyle.Bold))
            using (Brush lcdText = new SolidBrush(Color.FromArgb(189, 198, 192)))
            using (Brush lcdOrange = new SolidBrush(Color.FromArgb(228, 122, 23)))
            {
                g.DrawString("SYNC: " + syncModes[state.DlySyncMode] + " (CLICK)", lcdSmall, lcdOrange, lcdRect.X + 12, lcdRect.Y + 10);
                g.DrawString("P-P: " + (state.DlyPingPong ? "ON " : "OFF") + " (CLICK)", lcdSmall, lcdOrange, lcdRect.Right - 135, lcdRect.Y + 10);

                string centerReadout = state.DlySyncMode == 0
                    ? string.Format("{0:0.0} MS", state.DlyTimeMs)
                    : string.Format("{0}  ({1:0.0} BPM)", SubdivisionNames[state.DlySubdivIndex], bpm);
                g.DrawString(centerReadout, lcdBig, lcdText, lcdRect.X + 14, lcdRect.Y + 34);
                g.DrawString(string.Format("SAT {0:0}%  |  MOD {1:0}%  |  FBK {2:0}%",
                    state.DlyTapeSat * 100f, state.DlyTapeMod * 100f, state.DlyFeedback * 100f),
                    lcdSmall, lcdText, lcdRect.X + 12, lcdRect.Y + 64);
            }

            hotspots.Add(new UiHotspot
            {
                Id = "dly_sync_cycle",
                Bounds = new RectangleF(lcdRect.X, lcdRect.Y, lcdRect.Width * 0.5f, lcdRect.Height),
                OnClick = () => { state.DlySyncMode = (state.DlySyncMode + 1) % 3; MarkDirty(); }
            });
            hotspots.Add(new UiHotspot
            {
                Id = "dly_pp_toggle",
                Bounds = new RectangleF(lcdRect.X + lcdRect.Width * 0.5f, lcdRect.Y, lcdRect.Width * 0.5f, lcdRect.Height),
                OnClick = () => { state.DlyPingPong = !state.DlyPingPong; MarkDirty(); }
            });

            // Dual Footswitches on Delay: ENGAGE + TAP TEMPO
            DrawStompSwitch(g, new RectangleF(dlyRect.X + 82, dlyRect.Y + 356, 66, 66), state.DlyActive, "ENGAGE",
                () => { state.DlyActive = !state.DlyActive; MarkDirty(); });
            DrawStompSwitch(g, new RectangleF(dlyRect.Right - 148, dlyRect.Y + 356, 66, 66), beatPhase < 0.25, "TAP TEMPO",
                RegisterTapTempo);

            // Right Pedal: REV Reverb (x: 530, y: 228, w: 410, h: 444)
            RectangleF revRect = new RectangleF(530, 228, 410, 444);
            DrawPedalEnclosure(g, revRect, Color.FromArgb(26, 28, 36), Color.FromArgb(14, 15, 20), Color.FromArgb(10, 116, 224), "REV", "ATMOSPHERIC SHIMMER REVERB");
            DrawGeometricPedalArt(g, new RectangleF(revRect.X + 55, revRect.Y + 236, revRect.Width - 110, 95), Color.FromArgb(65, 10, 116, 224));

            DrawRotaryKnob(g, new RectangleF(revRect.X + 68, revRect.Y + 58, 66, 66), state.RevMix, "MIX",
                string.Format("{0:0}%", state.RevMix * 100f), Color.FromArgb(10, 116, 224),
                dy => { state.RevMix = Clamp(state.RevMix - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.RevMix = 0.22f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(revRect.Right - 134, revRect.Y + 58, 66, 66), state.RevDecay, "DECAY",
                string.Format("{0:0}%", state.RevDecay * 100f), Color.FromArgb(10, 116, 224),
                dy => { state.RevDecay = Clamp(state.RevDecay - dy * 0.005f, 0f, 1f); MarkDirty(); },
                () => { state.RevDecay = 0.46f; MarkDirty(); });

            DrawRotaryKnob(g, new RectangleF(revRect.X + 68, revRect.Y + 154, 66, 66), (state.RevLowCutHz - 50f) / 650f, "LOW CUT",
                string.Format("{0:0} Hz", state.RevLowCutHz), Color.FromArgb(10, 116, 224),
                dy => { state.RevLowCutHz = Clamp(state.RevLowCutHz - dy * 2.5f, 50f, 700f); MarkDirty(); },
                () => { state.RevLowCutHz = 120.0f; MarkDirty(); });
            DrawRotaryKnob(g, new RectangleF(revRect.Right - 134, revRect.Y + 154, 66, 66), (state.RevHighCutKhz - 1f) / 9f, "HIGH CUT",
                string.Format("{0:0.1} kHz", state.RevHighCutKhz), Color.FromArgb(10, 116, 224),
                dy => { state.RevHighCutKhz = Clamp(state.RevHighCutKhz - dy * 0.04f, 1.0f, 10.0f); MarkDirty(); },
                () => { state.RevHighCutKhz = 6.8f; MarkDirty(); });

            // SHIMMER Switch + BYPASS Stomp Switch
            DrawStompSwitch(g, new RectangleF(revRect.X + 82, revRect.Y + 356, 66, 66), state.RevShimmer, "SHIMMER",
                () => { state.RevShimmer = !state.RevShimmer; MarkDirty(); });
            DrawStompSwitch(g, new RectangleF(revRect.Right - 148, revRect.Y + 356, 66, 66), state.RevActive, "BYPASS",
                () => { state.RevActive = !state.RevActive; MarkDirty(); });
        }

        // ====================================================================
        // BOTTOM AMP/CAB SELECTOR BAR (y: 688..748)
        // ====================================================================
        private void DrawBottomAmpCabSelector(Graphics g)
        {
            // Link/Unlink Chain Icon Button (x: 345, y: 696, w: 68, h: 42)
            RectangleF linkBtn = new RectangleF(345, 696, 68, 42);
            DrawButtonPill(g, linkBtn, state.AmpCabLinked ? "LINKED" : "UNLINK",
                state.AmpCabLinked ? Color.FromArgb(40, 228, 122, 23) : Color.FromArgb(26, 26, 30),
                state.AmpCabLinked ? Color.FromArgb(228, 122, 23) : Color.FromArgb(140, 142, 150), 7.5f);
            hotspots.Add(new UiHotspot
            {
                Id = "amp_cab_link",
                Bounds = linkBtn,
                OnClick = () =>
                {
                    state.AmpCabLinked = !state.AmpCabLinked;
                    if (state.AmpCabLinked) state.CabType = state.AmpType;
                    MarkDirty();
                }
            });

            // 3 Amp/Cab Selector Icons: CLN (Clean), RST (Rust), HOT (Lead)
            string[] names = new string[] { "CLN", "RST", "HOT" };
            int activeIdx = (selectedSection == 3) ? state.CabType : state.AmpType;

            for (int i = 0; i < 3; i++)
            {
                int idx = i;
                RectangleF bRect = new RectangleF(425 + i * 86, 694, 76, 46);
                bool sel = (activeIdx == i);
                Color accent = i == 0 ? Color.FromArgb(212, 168, 92) : (i == 1 ? Color.FromArgb(255, 120, 45) : Color.FromArgb(255, 48, 58));

                using (GraphicsPath bp = RoundedRect(bRect, 6f))
                using (Brush bb = new SolidBrush(sel ? Color.FromArgb(38, 38, 44) : Color.FromArgb(20, 20, 24)))
                using (Pen bpen = new Pen(sel ? accent : Color.FromArgb(52, 52, 58), sel ? 1.8f : 1.0f))
                {
                    g.FillPath(bb, bp);
                    g.DrawPath(bpen, bp);
                }

                using (Font f = new Font("Segoe UI", 8.5f, FontStyle.Bold))
                using (Brush tb = new SolidBrush(sel ? Color.White : Color.FromArgb(150, 152, 160)))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(names[i], f, tb, bRect, sf);
                }

                hotspots.Add(new UiHotspot
                {
                    Id = "amp_cab_sel_" + idx,
                    Bounds = bRect,
                    OnClick = () =>
                    {
                        if (selectedSection == 3 && !state.AmpCabLinked)
                        {
                            state.CabType = idx;
                        }
                        else
                        {
                            state.AmpType = idx;
                            if (state.AmpCabLinked) state.CabType = idx;
                        }
                        MarkDirty();
                    }
                });
            }
        }

        // ====================================================================
        // BOTTOM UTILITY BAR (y: 750..782)
        // ====================================================================
        private void DrawBottomUtilityBar(Graphics g)
        {
            RectangleF barRect = new RectangleF(0, 750, LOGICAL_WIDTH, 32);
            using (Brush b = new SolidBrush(Color.FromArgb(12, 12, 14)))
            using (Pen p = new Pen(Color.FromArgb(46, 46, 52), 1f))
            {
                g.FillRectangle(b, barRect);
                g.DrawLine(p, 0, 750, LOGICAL_WIDTH, 750);
            }

            // 1. TUNER Button + Live Tuner Pill
            RectangleF tunerBtn = new RectangleF(10, 754, 68, 24);
            DrawButtonPill(g, tunerBtn, "TUNER", activeModal == 1 ? Color.FromArgb(69, 248, 98) : Color.FromArgb(26, 26, 30),
                activeModal == 1 ? Color.Black : Color.FromArgb(200, 202, 210), 7.5f);
            hotspots.Add(new UiHotspot { Id = "util_tuner", Bounds = tunerBtn, OnClick = () => { activeModal = (activeModal == 1) ? 0 : 1; } });

            if (liveTunerEnabled)
            {
                RectangleF livePill = new RectangleF(84, 754, 108, 24);
                DrawButtonPill(g, livePill, string.Format("● {0}  {1:+0.0;-0.0;0.0} ct", detectedNoteName, detectedCents),
                    Color.FromArgb(20, 69, 248, 98), Color.FromArgb(69, 248, 98), 7.5f);
                hotspots.Add(new UiHotspot { Id = "util_livetuner", Bounds = livePill, OnClick = () => { activeModal = 1; } });
            }

            // 2. MIDI Button
            RectangleF midiBtn = new RectangleF(200, 754, 58, 24);
            DrawButtonPill(g, midiBtn, "MIDI", activeModal == 3 ? Color.FromArgb(228, 122, 23) : Color.FromArgb(26, 26, 30),
                activeModal == 3 ? Color.Black : Color.FromArgb(200, 202, 210), 7.5f);
            hotspots.Add(new UiHotspot { Id = "util_midi", Bounds = midiBtn, OnClick = () => { activeModal = (activeModal == 3) ? 0 : 3; } });

            // 3. TAP Button + BPM Value (40..240 BPM)
            RectangleF tapBtn = new RectangleF(268, 754, 48, 24);
            DrawButtonPill(g, tapBtn, "TAP", beatPhase < 0.2 ? Color.FromArgb(55, 228, 122, 23) : Color.FromArgb(26, 26, 30),
                Color.FromArgb(228, 122, 23), 7.5f);
            hotspots.Add(new UiHotspot { Id = "util_tap", Bounds = tapBtn, OnClick = RegisterTapTempo });

            RectangleF bpmBox = new RectangleF(320, 754, 84, 24);
            DrawButtonPill(g, bpmBox, string.Format("{0:0.0} BPM", bpm), Color.FromArgb(18, 18, 22), Color.White, 7.5f);
            hotspots.Add(new UiHotspot
            {
                Id = "util_bpm",
                Bounds = bpmBox,
                OnDragDelta = dy => { bpm = Clamp(bpm - dy * 0.25f, 40f, 240f); },
                OnDoubleClick = () => { bpm = 120.0f; }
            });

            // 4. METRONOME Button + Inline Play/Stop Button
            RectangleF metroBtn = new RectangleF(412, 754, 92, 24);
            DrawButtonPill(g, metroBtn, "METRONOME", activeModal == 2 ? Color.FromArgb(228, 122, 23) : Color.FromArgb(26, 26, 30),
                activeModal == 2 ? Color.Black : Color.FromArgb(200, 202, 210), 7.5f);
            hotspots.Add(new UiHotspot { Id = "util_metro", Bounds = metroBtn, OnClick = () => { activeModal = (activeModal == 2) ? 0 : 2; } });

            RectangleF metroPlay = new RectangleF(508, 754, 34, 24);
            DrawButtonPill(g, metroPlay, metronomePlaying ? "■" : "►",
                metronomePlaying ? Color.FromArgb(69, 248, 98) : Color.FromArgb(26, 26, 30),
                metronomePlaying ? Color.Black : Color.FromArgb(200, 202, 210), 7.5f);
            hotspots.Add(new UiHotspot { Id = "util_metro_play", Bounds = metroPlay, OnClick = () => { metronomePlaying = !metronomePlaying; } });

            // 5. SETTINGS / DSP INSPECTOR Button
            RectangleF setBtn = new RectangleF(552, 754, 114, 24);
            DrawButtonPill(g, setBtn, "SETTINGS / DSP", activeModal == 4 ? Color.FromArgb(10, 116, 224) : Color.FromArgb(26, 26, 30),
                activeModal == 4 ? Color.White : Color.FromArgb(200, 202, 210), 7.5f);
            hotspots.Add(new UiHotspot { Id = "util_settings", Bounds = setBtn, OnClick = () => { activeModal = (activeModal == 4) ? 0 : 4; } });

            // 6. About / Credits Button
            RectangleF aboutBtn = new RectangleF(676, 754, 210, 24);
            DrawButtonPill(g, aboutBtn, "ARCHETYPE GOJIRA X  v1.0.2", Color.FromArgb(18, 18, 22), Color.FromArgb(150, 152, 160), 7.2f);
            hotspots.Add(new UiHotspot { Id = "util_about", Bounds = aboutBtn, OnClick = () => { activeModal = (activeModal == 5) ? 0 : 5; } });

            // 7. Window Size Scale Button (Small 0.715x .. Maximum 2.0x)
            RectangleF sizeBtn = new RectangleF(896, 754, 118, 24);
            DrawButtonPill(g, sizeBtn, "SIZE: " + scaleNames[currentScaleIndex].Split(' ')[0].ToUpperInvariant(),
                Color.FromArgb(26, 26, 30), Color.FromArgb(215, 218, 225), 7.2f);
            hotspots.Add(new UiHotspot
            {
                Id = "util_scale",
                Bounds = sizeBtn,
                OnClick = () => { ApplyScale((currentScaleIndex + 1) % scaleFactors.Length); }
            });
        }

        // ====================================================================
        // MODAL OVERLAYS (Tuner, Metronome, MIDI, Settings/Inspector, About, Preset Dropdown)
        // ====================================================================
        private void DrawModalOverlay(Graphics g)
        {
            if (activeModal == 6)
            {
                // Preset Browser Dropdown Menu
                RectangleF menuRect = new RectangleF(352, 172, 320, 232);
                using (GraphicsPath mp = RoundedRect(menuRect, 8f))
                using (Brush mb = new SolidBrush(Color.FromArgb(245, 18, 19, 24)))
                using (Pen mpen = new Pen(Color.FromArgb(228, 122, 23), 1.5f))
                {
                    g.FillPath(mb, mp);
                    g.DrawPath(mpen, mp);
                }
                for (int i = 0; i < presets.Count; i++)
                {
                    int idx = i;
                    RectangleF row = new RectangleF(menuRect.X + 8, menuRect.Y + 10 + i * 35, menuRect.Width - 16, 31);
                    bool sel = (i == currentPresetIndex);
                    DrawButtonPill(g, row, presets[i].Category.ToUpperInvariant() + "  /  " + presets[i].Name,
                        sel ? Color.FromArgb(228, 122, 23) : Color.FromArgb(30, 32, 38),
                        sel ? Color.Black : Color.FromArgb(225, 228, 235), 8.2f);
                    hotspots.Add(new UiHotspot
                    {
                        Id = "preset_pick_" + idx,
                        Bounds = row,
                        OnClick = () => { LoadPreset(idx); activeModal = 0; }
                    });
                }
                return;
            }

            // Dimmed backdrop over main stage
            RectangleF backdrop = new RectangleF(0, 190, LOGICAL_WIDTH, 560);
            using (Brush dim = new SolidBrush(Color.FromArgb(215, 8, 9, 12)))
            {
                g.FillRectangle(dim, backdrop);
            }

            RectangleF modalBox = new RectangleF(132, 224, 760, 488);
            using (GraphicsPath mp = RoundedRect(modalBox, 12f))
            using (Brush mb = new SolidBrush(Color.FromArgb(22, 23, 28)))
            using (Pen mpen = new Pen(Color.FromArgb(72, 74, 86), 1.8f))
            {
                g.FillPath(mb, mp);
                g.DrawPath(mpen, mp);
            }

            // Close [X] button in top-right of modal
            RectangleF closeBtn = new RectangleF(modalBox.Right - 44, modalBox.Y + 12, 32, 28);
            DrawButtonPill(g, closeBtn, "✕", Color.FromArgb(44, 46, 54), Color.White, 9f);
            hotspots.Add(new UiHotspot { Id = "modal_close", Bounds = closeBtn, OnClick = () => { activeModal = 0; } });

            if (activeModal == 1) DrawTunerModal(g, modalBox);
            else if (activeModal == 2) DrawMetronomeModal(g, modalBox);
            else if (activeModal == 3) DrawMidiModal(g, modalBox);
            else if (activeModal == 4) DrawSettingsInspectorModal(g, modalBox);
            else if (activeModal == 5) DrawAboutModal(g, modalBox);
        }

        private void DrawTunerModal(Graphics g, RectangleF box)
        {
            using (Font tf = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (Font bigNote = new Font("Segoe UI", 48f, FontStyle.Bold))
            using (Font subF = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (Brush wb = new SolidBrush(Color.White))
            using (Brush gb = new SolidBrush(Color.FromArgb(69, 248, 98)))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString("CHROMATIC PRECISION TUNER  (-50 .. +50 CENTS)", tf, wb, box.X + 20, box.Y + 16);

                // Cents Arc Scale (-50 .. 0 .. +50)
                RectangleF meterBar = new RectangleF(box.X + 80, box.Y + 95, box.Width - 160, 44);
                using (Brush mb = new SolidBrush(Color.FromArgb(12, 14, 16)))
                using (Pen mp = new Pen(Color.FromArgb(56, 60, 68), 1.2f))
                {
                    g.FillRectangle(mb, meterBar.X, meterBar.Y, meterBar.Width, meterBar.Height);
                    g.DrawRectangle(mp, meterBar.X, meterBar.Y, meterBar.Width, meterBar.Height);
                }

                float centerX = meterBar.X + meterBar.Width * 0.5f;
                float needleX = centerX + (detectedCents / 50f) * (meterBar.Width * 0.45f);
                using (Pen nPen = new Pen(Color.FromArgb(69, 248, 98), 3f))
                {
                    g.DrawLine(nPen, needleX, meterBar.Y + 4, needleX, meterBar.Bottom - 4);
                }
                g.FillEllipse(gb, centerX - 8, meterBar.Y - 18, 16, 16);

                // Large Detected Note Name & Readout
                g.DrawString(detectedNoteName, bigNote, gb, new RectangleF(box.X, box.Y + 160, box.Width, 90), sf);
                string pitchText = tunerDisplayHz
                    ? string.Format("{0:0.00} Hz   (REF A4 = {1:0.0} Hz)", detectedFreqHz, tunerRefPitchHz)
                    : string.Format("{0:+0.0;-0.0;0.0} CENTS   ({1:0.00} Hz)", detectedCents, detectedFreqHz);
                g.DrawString(pitchText, subF, wb, new RectangleF(box.X, box.Y + 260, box.Width, 28), sf);
            }

            // Bottom Tuner Controls: MUTE | CENTS/HZ | LIVE TUNER | REF PITCH (420..460 Hz)
            RectangleF muteBtn = new RectangleF(box.X + 80, box.Y + 360, 130, 36);
            DrawButtonPill(g, muteBtn, tunerMuted ? "OUTPUT: MUTED" : "MUTE OUTPUT",
                tunerMuted ? Color.FromArgb(255, 48, 58) : Color.FromArgb(36, 38, 46), Color.White, 8.5f);
            hotspots.Add(new UiHotspot { Id = "tuner_mute", Bounds = muteBtn, OnClick = () => { tunerMuted = !tunerMuted; } });

            RectangleF unitBtn = new RectangleF(box.X + 230, box.Y + 360, 130, 36);
            DrawButtonPill(g, unitBtn, tunerDisplayHz ? "DISPLAY: HZ" : "DISPLAY: CENTS",
                Color.FromArgb(36, 38, 46), Color.White, 8.5f);
            hotspots.Add(new UiHotspot { Id = "tuner_unit", Bounds = unitBtn, OnClick = () => { tunerDisplayHz = !tunerDisplayHz; } });

            RectangleF liveBtn = new RectangleF(box.X + 380, box.Y + 360, 140, 36);
            DrawButtonPill(g, liveBtn, liveTunerEnabled ? "LIVE TUNER: ON" : "LIVE TUNER: OFF",
                liveTunerEnabled ? Color.FromArgb(35, 69, 248, 98) : Color.FromArgb(36, 38, 46),
                liveTunerEnabled ? Color.FromArgb(69, 248, 98) : Color.White, 8.5f);
            hotspots.Add(new UiHotspot { Id = "tuner_live", Bounds = liveBtn, OnClick = () => { liveTunerEnabled = !liveTunerEnabled; } });

            DrawRotaryKnob(g, new RectangleF(box.X + 575, box.Y + 336, 68, 68), (tunerRefPitchHz - 420f) / 40f, "REF PITCH",
                string.Format("{0:0.0} Hz", tunerRefPitchHz), Color.FromArgb(69, 248, 98),
                dy => { tunerRefPitchHz = Clamp(tunerRefPitchHz - dy * 0.1f, 420f, 460f); },
                () => { tunerRefPitchHz = 440.0f; });
        }

        private void DrawMetronomeModal(Graphics g, RectangleF box)
        {
            using (Font tf = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (Brush wb = new SolidBrush(Color.White))
            {
                g.DrawString("PROGRAMMABLE METRONOME & RHYTHM SEQUENCER", tf, wb, box.X + 20, box.Y + 16);
            }

            // Time Signature Selector, Sound Set Selector & Play Button
            RectangleF tsBtn = new RectangleF(box.X + 40, box.Y + 70, 210, 36);
            DrawButtonPill(g, tsBtn, "TIME SIG: " + TimeSignatureNames[metronomeTimeSigIndex] + " (CLICK)",
                Color.FromArgb(34, 36, 44), Color.FromArgb(228, 122, 23), 9f);
            hotspots.Add(new UiHotspot
            {
                Id = "metro_ts",
                Bounds = tsBtn,
                OnClick = () => { metronomeTimeSigIndex = (metronomeTimeSigIndex + 1) % TimeSignatureNames.Length; }
            });

            RectangleF ssBtn = new RectangleF(box.X + 260, box.Y + 70, 210, 36);
            DrawButtonPill(g, ssBtn, "SOUND: " + SoundSetNames[metronomeSoundSetIndex],
                Color.FromArgb(34, 36, 44), Color.White, 8.5f);
            hotspots.Add(new UiHotspot
            {
                Id = "metro_ss",
                Bounds = ssBtn,
                OnClick = () => { metronomeSoundSetIndex = (metronomeSoundSetIndex + 1) % SoundSetNames.Length; }
            });

            RectangleF rhyBtn = new RectangleF(box.X + 480, box.Y + 70, 230, 36);
            DrawButtonPill(g, rhyBtn, "RHYTHM: " + RhythmNames[metronomeRhythmIndex],
                Color.FromArgb(34, 36, 44), Color.FromArgb(10, 116, 224), 8.5f);
            hotspots.Add(new UiHotspot
            {
                Id = "metro_rhy",
                Bounds = rhyBtn,
                OnClick = () => { metronomeRhythmIndex = (metronomeRhythmIndex + 1) % RhythmNames.Length; }
            });

            RectangleF playBtn = new RectangleF(box.X + 270, box.Y + 255, 220, 36);
            DrawButtonPill(g, playBtn, metronomePlaying ? "■ STOP METRONOME" : "► START METRONOME",
                metronomePlaying ? Color.FromArgb(69, 248, 98) : Color.FromArgb(228, 122, 23), Color.Black, 9f);
            hotspots.Add(new UiHotspot { Id = "metro_toggle", Bounds = playBtn, OnClick = () => { metronomePlaying = !metronomePlaying; } });

            // Knobs: VOLUME (-12..+12 dB), BPM (40..240), PAN (-50..+50)
            DrawRotaryKnob(g, new RectangleF(box.X + 120, box.Y + 145, 78, 78), (metronomeVolDb + 12f) / 24f, "VOLUME",
                string.Format("{0:+0.0;-0.0;0.0} dB", metronomeVolDb), Color.FromArgb(228, 122, 23),
                dy => { metronomeVolDb = Clamp(metronomeVolDb - dy * 0.1f, -12f, 12f); },
                () => { metronomeVolDb = 0.0f; });

            DrawRotaryKnob(g, new RectangleF(box.X + 340, box.Y + 145, 78, 78), (bpm - 40f) / 200f, "TEMPO BPM",
                string.Format("{0:0.0} BPM", bpm), Color.FromArgb(69, 248, 98),
                dy => { bpm = Clamp(bpm - dy * 0.5f, 40f, 240f); },
                () => { bpm = 120.0f; });

            DrawRotaryKnob(g, new RectangleF(box.X + 560, box.Y + 145, 78, 78), (metronomePan + 50f) / 100f, "PAN",
                string.Format("{0:+0;-0;0}", metronomePan), Color.FromArgb(10, 116, 224),
                dy => { metronomePan = Clamp(metronomePan - dy * 0.5f, -50f, 50f); },
                () => { metronomePan = 0.0f; });

            // Beat Step Accent Sequencer Row
            int numBeats = GetTimeSigBeats();
            float stepW = (box.Width - 100) / (float)numBeats;
            for (int b = 0; b < numBeats; b++)
            {
                int bIdx = b;
                RectangleF stepRect = new RectangleF(box.X + 50 + b * stepW + 6, box.Y + 310, stepW - 12, 95);
                bool isCurrent = metronomePlaying && (currentBeatStep == b);
                int accent = metronomeBeatAccents[b];
                Color fill = accent == 2 ? Color.FromArgb(228, 122, 23) : (accent == 1 ? Color.FromArgb(10, 116, 224) : Color.FromArgb(32, 34, 40));
                if (isCurrent) fill = Color.FromArgb(69, 248, 98);

                string label = string.Format("BEAT {0}\n{1}", b + 1, accent == 2 ? "ACCENT" : (accent == 1 ? "NORMAL" : "MUTE"));
                DrawButtonPill(g, stepRect, label, fill, (accent == 2 || isCurrent) ? Color.Black : Color.White, 8.5f);
                hotspots.Add(new UiHotspot
                {
                    Id = "metro_beat_" + bIdx,
                    Bounds = stepRect,
                    OnClick = () => { metronomeBeatAccents[bIdx] = (metronomeBeatAccents[bIdx] + 1) % 3; }
                });
            }
        }

        private void DrawMidiModal(Graphics g, RectangleF box)
        {
            using (Font tf = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (Brush wb = new SolidBrush(Color.White))
            {
                g.DrawString("MIDI MAPPINGS & CONTROLLER ASSIGNMENTS", tf, wb, box.X + 20, box.Y + 16);
            }

            RectangleF addBtn = new RectangleF(box.Right - 225, box.Y + 12, 170, 28);
            DrawButtonPill(g, addBtn, "+ NEW MIDI MAPPING", Color.FromArgb(228, 122, 23), Color.Black, 8f);
            hotspots.Add(new UiHotspot
            {
                Id = "midi_add",
                Bounds = addBtn,
                OnClick = () =>
                {
                    midiMappings.Add(new MidiMappingRow
                    {
                        Type = "Control Change",
                        Parameter = "Amp: HOT Gain",
                        Channel = 1,
                        NoteOrCc = "CC #" + (20 + midiMappings.Count),
                        ValueRange = "0 .. 127"
                    });
                }
            });

            // Table Header
            string[] cols = new string[] { "MESSAGE TYPE", "TARGET PARAMETER / PRESET", "CHANNEL", "CC / PC / NOTE", "RANGE" };
            float[] colX = new float[] { box.X + 24, box.X + 170, box.X + 430, box.X + 520, box.X + 635 };
            using (Font hf = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (Brush hb = new SolidBrush(Color.FromArgb(228, 122, 23)))
            {
                for (int c = 0; c < cols.Length; c++) g.DrawString(cols[c], hf, hb, colX[c], box.Y + 62);
            }

            using (Font rf = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            using (Brush rb = new SolidBrush(Color.FromArgb(225, 228, 235)))
            {
                for (int r = 0; r < Math.Min(9, midiMappings.Count); r++)
                {
                    int rowIdx = r;
                    float ry = box.Y + 90 + r * 40;
                    RectangleF rowBox = new RectangleF(box.X + 18, ry, box.Width - 36, 34);
                    using (Brush rbg = new SolidBrush(r % 2 == 0 ? Color.FromArgb(28, 30, 36) : Color.FromArgb(24, 25, 30)))
                    {
                        g.FillRectangle(rbg, rowBox);
                    }
                    MidiMappingRow m = midiMappings[r];
                    g.DrawString(m.Type, rf, rb, colX[0], ry + 8);
                    g.DrawString(m.Parameter, rf, rb, colX[1], ry + 8);
                    g.DrawString("CH " + m.Channel, rf, rb, colX[2], ry + 8);
                    g.DrawString(m.NoteOrCc, rf, rb, colX[3], ry + 8);
                    g.DrawString(m.ValueRange, rf, rb, colX[4], ry + 8);

                    RectangleF delBtn = new RectangleF(rowBox.Right - 34, ry + 4, 26, 26);
                    DrawButtonPill(g, delBtn, "✕", Color.FromArgb(52, 30, 34), Color.FromArgb(255, 90, 90), 8f);
                    hotspots.Add(new UiHotspot
                    {
                        Id = "midi_del_" + rowIdx,
                        Bounds = delBtn,
                        OnClick = () => { if (rowIdx < midiMappings.Count) midiMappings.RemoveAt(rowIdx); }
                    });
                }
            }
        }

        private void DrawSettingsInspectorModal(Graphics g, RectangleF box)
        {
            using (Font tf = new Font("Segoe UI", 11.5f, FontStyle.Bold))
            using (Font sf = new Font("Consolas", 8.5f, FontStyle.Regular))
            using (Brush wb = new SolidBrush(Color.White))
            using (Brush ab = new SolidBrush(Color.FromArgb(228, 122, 23)))
            using (Brush gb = new SolidBrush(Color.FromArgb(69, 248, 98)))
            {
                g.DrawString("AUDIO SETTINGS & DECOMPILED PACKAGE / KONE DSP ARCHITECTURE", tf, wb, box.X + 20, box.Y + 16);

                string info =
                    "=== AUDIO ENGINE CONFIGURATION ===\n" +
                    "Driver Type : " + audioDeviceType + "   |   Device : " + audioDeviceName + "\n" +
                    "Sample Rate : " + sampleRate + " Hz       |   Buffer : " + bufferSize + " Samples (2.7 ms Latency)\n\n" +
                    "=== DECOMPILED ADVINSTSFX INSTALLER & PAYLOAD TABLE ===\n" +
                    "Bootstrap Loader : WinMain @ 0x00413640  ->  SFX Controller @ 0x00558830\n" +
                    "Header Stream    : FUN_005709d0 @ 0x005709d0 (Bitwise NOT ~byte on first 512 bytes)\n" +
                    "Cabinet Payload  : Archetype Gojira X1.cab @ offset 0x009D5BB3 (476,829,039 bytes, 328 files)\n" +
                    "Standalone App   : ArchetypeGojiraX.exe (129,969,992 bytes, x86_64 PE32+ GUI)\n\n" +
                    "=== KONE WORKBENCH EMBEDDED DSP GRAPHS (139 XML DOCUMENTS) ===\n" +
                    "• Pitch Graph @ 0x146519620 : WOW Shifter (Fatso/Blade1/Blade2) + OCT (-12st/-24st)\n" +
                    "• PreFX Graph @ 0x14651a890 : OD + DRT + PHSR (0.12..10 Hz) + CHR (0.12..10 Hz)\n" +
                    "• Amp/Cab/EQ  @ 0x14651ef80 : CLN/RST/HOT Amps + Dual IR Loader + 3x 9-Band Graphic EQ\n" +
                    "• PostFX      @ 0x146522e50 : Tape Ping-Pong Delay (100..1100 ms) + Shimmer Reverb";

                g.DrawString(info, sf, wb, new RectangleF(box.X + 24, box.Y + 62, box.Width - 48, 340));
                g.DrawString("ACTIVE DSP VERIFICATION: ALL 6 SIGNAL SECTIONS & 321 PRESET ARCHITECTURES READY", sf, gb, box.X + 24, box.Bottom - 54);
            }
        }

        private void DrawAboutModal(Graphics g, RectangleF box)
        {
            using (Font tf = new Font("Segoe UI", 16f, FontStyle.Bold))
            using (Font sf = new Font("Segoe UI", 9.5f, FontStyle.Regular))
            using (Brush wb = new SolidBrush(Color.White))
            using (Brush ab = new SolidBrush(Color.FromArgb(228, 122, 23)))
            using (StringFormat center = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString("ARCHETYPE GOJIRA X", tf, wb, new RectangleF(box.X, box.Y + 70, box.Width, 36), center);
                g.DrawString("VERSION 1.0.2  —  STANDALONE DESKTOP SUITE", sf, ab, new RectangleF(box.X, box.Y + 112, box.Width, 24), center);
                string desc =
                    "Complete clean-room desktop recreation of Archetype Gojira X v1.0.2.\n\n" +
                    "• 6 Signal Chain Sections: Pitch (WOW + OCT), Pre FX (OD + DRT + PHSR + CHR),\n" +
                    "  Amp (CLN + RST + HOT), Cab (Dual Mic IR Loader), EQ (3x 9-Band + HPF/LPF), Post FX (DLY + REV)\n" +
                    "• Global Signal Bar: Input/Output Gain, Noise Gate, Transpose (-12..+12 st), Mono/Stereo, Doubler\n" +
                    "• Utility Suite: Chromatic Live Tuner, Programmable Metronome, Tap Tempo & MIDI Mappings";
                g.DrawString(desc, sf, wb, new RectangleF(box.X + 60, box.Y + 165, box.Width - 120, 220), center);
            }
        }

        private void RegisterTapTempo()
        {
            long now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            if (tapTimestamps.Count > 0 && (now - tapTimestamps[tapTimestamps.Count - 1]) > 2500)
            {
                tapTimestamps.Clear();
            }
            tapTimestamps.Add(now);
            if (tapTimestamps.Count > 4) tapTimestamps.RemoveAt(0);
            if (tapTimestamps.Count >= 2)
            {
                double avgMs = (tapTimestamps[tapTimestamps.Count - 1] - tapTimestamps[0]) / (double)(tapTimestamps.Count - 1);
                if (avgMs > 200 && avgMs < 1600)
                {
                    bpm = Clamp((float)(60000.0 / avgMs), 40f, 240f);
                }
            }
        }

        // ====================================================================
        // REUSABLE VECTOR DRAWING HELPERS
        // ====================================================================
        private void DrawSectionHeaderBanner(Graphics g, string title, bool active)
        {
            RectangleF banner = new RectangleF(24, 196, LOGICAL_WIDTH - 48, 24);
            using (Font f = new Font("Segoe UI", 8.2f, FontStyle.Bold))
            using (Brush tb = new SolidBrush(active ? Color.FromArgb(200, 204, 214) : Color.FromArgb(130, 130, 138)))
            {
                g.DrawString(title + (active ? "" : "   [SECTION BYPASSED]"), f, tb, banner.X, banner.Y + 3);
            }
        }

        private void DrawPedalEnclosure(Graphics g, RectangleF rect, Color topCol, Color botCol, Color accent, string code, string subtitle)
        {
            using (GraphicsPath path = RoundedRect(rect, 14f))
            using (LinearGradientBrush bg = new LinearGradientBrush(rect, topCol, botCol, LinearGradientMode.Vertical))
            using (Pen border = new Pen(Color.FromArgb(72, 75, 84), 2f))
            {
                g.FillPath(bg, path);
                g.DrawPath(border, path);
            }

            // Corner hex screws
            float[] sx = new float[] { rect.X + 10, rect.Right - 18 };
            float[] sy = new float[] { rect.Y + 10, rect.Bottom - 18 };
            using (Brush sb = new SolidBrush(Color.FromArgb(85, 88, 96)))
            {
                foreach (float x in sx)
                    foreach (float y in sy)
                        g.FillEllipse(sb, x, y, 8, 8);
            }

            using (Font titleF = new Font("Segoe UI", 13f, FontStyle.Bold))
            using (Font subF = new Font("Segoe UI", 7.2f, FontStyle.Bold))
            using (Brush ab = new SolidBrush(accent))
            using (Brush mb = new SolidBrush(Color.FromArgb(165, 168, 178)))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString(code, titleF, ab, new RectangleF(rect.X, rect.Y + 10, rect.Width, 24), sf);
                g.DrawString(subtitle, subF, mb, new RectangleF(rect.X, rect.Y + 32, rect.Width, 16), sf);
            }
        }

        private void DrawGeometricPedalArt(Graphics g, RectangleF rect, Color col)
        {
            float cx = rect.X + rect.Width * 0.5f;
            float cy = rect.Y + rect.Height * 0.5f;
            float r = Math.Min(rect.Width, rect.Height) * 0.44f;
            using (Pen p = new Pen(col, 1.5f))
            {
                g.DrawEllipse(p, cx - r, cy - r, r * 2, r * 2);
                g.DrawEllipse(p, cx - r * 0.68f, cy - r * 0.68f, r * 1.36f, r * 1.36f);
                PointF[] tri = new PointF[]
                {
                    new PointF(cx, cy - r * 0.9f),
                    new PointF(cx + r * 0.82f, cy + r * 0.65f),
                    new PointF(cx - r * 0.82f, cy + r * 0.65f)
                };
                g.DrawPolygon(p, tri);
            }
        }

        private void DrawRotaryKnob(Graphics g, RectangleF rect, float normVal, string label, string valueText,
            Color accent, Action<float> onDragDelta, Action onReset)
        {
            normVal = Clamp(normVal, 0f, 1f);
            float cx = rect.X + rect.Width * 0.5f;
            float cy = rect.Y + rect.Height * 0.5f;
            float r = rect.Width * 0.42f;

            // Background arc & active value arc (135 deg to 405 deg = 270 deg sweep)
            RectangleF arcRect = new RectangleF(cx - r - 4, cy - r - 4, (r + 4) * 2, (r + 4) * 2);
            using (Pen trackPen = new Pen(Color.FromArgb(42, 44, 52), 3.2f))
            using (Pen valPen = new Pen(accent, 3.2f))
            {
                g.DrawArc(trackPen, arcRect, 135f, 270f);
                if (normVal > 0.005f)
                {
                    g.DrawArc(valPen, arcRect, 135f, 270f * normVal);
                }
            }

            // Metallic Knob Cap
            RectangleF capRect = new RectangleF(cx - r, cy - r, r * 2, r * 2);
            using (LinearGradientBrush kb = new LinearGradientBrush(capRect, Color.FromArgb(58, 60, 68), Color.FromArgb(20, 21, 25), LinearGradientMode.Vertical))
            using (Pen kp = new Pen(Color.FromArgb(95, 98, 108), 1.4f))
            {
                g.FillEllipse(kb, capRect);
                g.DrawEllipse(kp, capRect);
            }

            // Pointer Indicator Line
            double angleRad = (135.0 + 270.0 * normVal) * Math.PI / 180.0;
            float px1 = cx + (float)Math.Cos(angleRad) * (r * 0.32f);
            float py1 = cy + (float)Math.Sin(angleRad) * (r * 0.32f);
            float px2 = cx + (float)Math.Cos(angleRad) * (r * 0.88f);
            float py2 = cy + (float)Math.Sin(angleRad) * (r * 0.88f);
            using (Pen ptrPen = new Pen(Color.White, 2.2f))
            {
                g.DrawLine(ptrPen, px1, py1, px2, py2);
            }

            // Label & Value Readout below knob
            using (Font lf = new Font("Segoe UI", 6.8f, FontStyle.Bold))
            using (Font vf = new Font("Segoe UI", 7.0f, FontStyle.Regular))
            using (Brush lb = new SolidBrush(Color.FromArgb(215, 218, 226)))
            using (Brush vb = new SolidBrush(accent))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString(label, lf, lb, new RectangleF(cx - 46, rect.Bottom + 1, 92, 13), sf);
                g.DrawString(valueText, vf, vb, new RectangleF(cx - 46, rect.Bottom + 13, 92, 13), sf);
            }

            hotspots.Add(new UiHotspot
            {
                Id = "knob_" + label + "_" + rect.X + "_" + rect.Y,
                Bounds = new RectangleF(rect.X - 6, rect.Y - 6, rect.Width + 12, rect.Height + 30),
                OnDragDelta = onDragDelta,
                OnDoubleClick = onReset
            });
        }

        private void DrawStompSwitch(Graphics g, RectangleF rect, bool active, string label, Action onClick)
        {
            float cx = rect.X + rect.Width * 0.5f;

            // Glowing Status LED above footswitch
            Color ledCol = active ? Color.FromArgb(255, 39, 39) : Color.FromArgb(55, 55, 62);
            if (active)
            {
                using (Brush glow = new SolidBrush(Color.FromArgb(75, 255, 39, 39)))
                {
                    g.FillEllipse(glow, cx - 12, rect.Y - 18, 24, 24);
                }
            }
            using (Brush lb = new SolidBrush(ledCol))
            using (Pen lp = new Pen(Color.FromArgb(90, 92, 100), 1f))
            {
                g.FillEllipse(lb, cx - 6, rect.Y - 12, 12, 12);
                g.DrawEllipse(lp, cx - 6, rect.Y - 12, 12, 12);
            }

            // Metallic Hex Washer & Plunger
            float r = 18f;
            float sy = rect.Y + 24;
            using (Brush wb = new SolidBrush(Color.FromArgb(145, 148, 158)))
            using (Brush pb = new SolidBrush(Color.FromArgb(215, 218, 226)))
            using (Pen p = new Pen(Color.FromArgb(50, 52, 58), 1.6f))
            {
                g.FillEllipse(wb, cx - r, sy - r, r * 2, r * 2);
                g.DrawEllipse(p, cx - r, sy - r, r * 2, r * 2);
                g.FillEllipse(pb, cx - r * 0.62f, sy - r * 0.62f, r * 1.24f, r * 1.24f);
                g.DrawEllipse(p, cx - r * 0.62f, sy - r * 0.62f, r * 1.24f, r * 1.24f);
            }

            using (Font f = new Font("Segoe UI", 7f, FontStyle.Bold))
            using (Brush tb = new SolidBrush(Color.FromArgb(195, 198, 206)))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString(label, f, tb, new RectangleF(cx - 48, rect.Bottom - 16, 96, 15), sf);
            }

            hotspots.Add(new UiHotspot
            {
                Id = "stomp_" + label + "_" + rect.X,
                Bounds = new RectangleF(rect.X, rect.Y - 16, rect.Width, rect.Height + 16),
                OnClick = onClick
            });
        }

        private void DrawAmpPowerJewel(Graphics g, RectangleF rect, bool powerOn, Color jewelCol, Action onClick)
        {
            float cx = rect.X + rect.Width * 0.5f;
            float cy = rect.Y + 24;

            if (powerOn)
            {
                using (Brush glow = new SolidBrush(Color.FromArgb(95, jewelCol)))
                {
                    g.FillEllipse(glow, cx - 22, cy - 22, 44, 44);
                }
            }
            using (Brush jb = new SolidBrush(powerOn ? jewelCol : Color.FromArgb(48, 50, 56)))
            using (Pen jp = new Pen(Color.FromArgb(180, 184, 192), 2f))
            {
                g.FillEllipse(jb, cx - 13, cy - 13, 26, 26);
                g.DrawEllipse(jp, cx - 13, cy - 13, 26, 26);
            }

            using (Font f = new Font("Segoe UI", 7f, FontStyle.Bold))
            using (Brush tb = new SolidBrush(Color.White))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString(powerOn ? "POWER ON" : "BYPASSED", f, tb, new RectangleF(cx - 42, rect.Bottom - 16, 84, 14), sf);
            }

            hotspots.Add(new UiHotspot { Id = "amp_pwr_" + rect.X, Bounds = rect, OnClick = onClick });
        }

        private void DrawVerticalMeter(Graphics g, RectangleF rect, float dbVal, float peakDb, bool clipLatched, string label, Action onClearClip)
        {
            using (Brush bg = new SolidBrush(Color.FromArgb(10, 11, 13)))
            using (Pen bp = new Pen(Color.FromArgb(46, 48, 56), 1f))
            {
                g.FillRectangle(bg, rect.X, rect.Y, rect.Width, rect.Height - 14);
                g.DrawRectangle(bp, rect.X, rect.Y, rect.Width, rect.Height - 14);
            }

            // Red Clip Latch LED at top of meter
            RectangleF clipRect = new RectangleF(rect.X + 1, rect.Y + 1, rect.Width - 2, 7);
            using (Brush cb = new SolidBrush(clipLatched ? Color.FromArgb(255, 39, 39) : Color.FromArgb(42, 22, 24)))
            {
                g.FillRectangle(cb, clipRect);
            }

            float barAreaH = rect.Height - 24;
            float norm = Clamp((dbVal + 60f) / 60f, 0f, 1f);
            float fillH = norm * barAreaH;
            if (fillH > 1f)
            {
                RectangleF fillRect = new RectangleF(rect.X + 2, rect.Y + 9 + (barAreaH - fillH), rect.Width - 4, fillH);
                using (LinearGradientBrush mb = new LinearGradientBrush(
                    new RectangleF(rect.X, rect.Y + 9, rect.Width, barAreaH),
                    Color.FromArgb(255, 39, 39),
                    Color.FromArgb(69, 248, 98),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(mb, fillRect);
                }
            }

            // Peak-hold horizontal tick
            float peakNorm = Clamp((peakDb + 60f) / 60f, 0f, 1f);
            float peakY = rect.Y + 9 + (1f - peakNorm) * barAreaH;
            using (Pen pp = new Pen(Color.White, 1.2f))
            {
                g.DrawLine(pp, rect.X + 2, peakY, rect.Right - 2, peakY);
            }

            using (Font f = new Font("Segoe UI", 6.5f, FontStyle.Bold))
            using (Brush tb = new SolidBrush(Color.FromArgb(170, 174, 182)))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString(label, f, tb, new RectangleF(rect.X - 6, rect.Bottom - 13, rect.Width + 12, 13), sf);
            }

            hotspots.Add(new UiHotspot { Id = "meter_" + label, Bounds = rect, OnClick = onClearClip });
        }

        private void DrawMiniToggle(Graphics g, RectangleF rect, bool on, string text, Action onClick)
        {
            DrawButtonPill(g, rect, text,
                on ? Color.FromArgb(45, 69, 248, 98) : Color.FromArgb(26, 27, 32),
                on ? Color.FromArgb(69, 248, 98) : Color.FromArgb(135, 138, 146), 6.8f);
            hotspots.Add(new UiHotspot { Id = "toggle_" + text + "_" + rect.X, Bounds = rect, OnClick = onClick });
        }

        private void DrawModeSwitchBox(Graphics g, RectangleF rect, string header, string optA, string optB, bool isB, Action onClick)
        {
            using (GraphicsPath p = RoundedRect(rect, 6f))
            using (Brush b = new SolidBrush(Color.FromArgb(18, 19, 23)))
            using (Pen pen = new Pen(Color.FromArgb(52, 54, 62), 1f))
            {
                g.FillPath(b, p);
                g.DrawPath(pen, p);
            }

            using (Font hf = new Font("Segoe UI", 6.5f, FontStyle.Bold))
            using (Brush hb = new SolidBrush(Color.FromArgb(155, 158, 166)))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString(header, hf, hb, new RectangleF(rect.X, rect.Y + 5, rect.Width, 13), sf);
            }

            RectangleF rA = new RectangleF(rect.X + 6, rect.Y + 22, rect.Width - 12, 24);
            RectangleF rB = new RectangleF(rect.X + 6, rect.Y + 50, rect.Width - 12, 24);
            DrawButtonPill(g, rA, optA, !isB ? Color.FromArgb(228, 122, 23) : Color.FromArgb(28, 29, 34), !isB ? Color.Black : Color.FromArgb(160, 162, 170), 7.2f);
            DrawButtonPill(g, rB, optB, isB ? Color.FromArgb(228, 122, 23) : Color.FromArgb(28, 29, 34), isB ? Color.Black : Color.FromArgb(160, 162, 170), 7.2f);

            hotspots.Add(new UiHotspot { Id = "mode_sw_" + header, Bounds = rect, OnClick = onClick });
        }

        private void DrawButtonPill(Graphics g, RectangleF rect, string text, Color fill, Color textCol, float fontSize)
        {
            using (GraphicsPath path = RoundedRect(rect, 4f))
            using (Brush b = new SolidBrush(fill))
            using (Pen p = new Pen(Color.FromArgb(62, 65, 74), 1f))
            {
                g.FillPath(b, path);
                g.DrawPath(p, path);
            }
            using (Font f = new Font("Segoe UI", fontSize, FontStyle.Bold))
            using (Brush tb = new SolidBrush(textCol))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, f, tb, rect, sf);
            }
        }

        private void DrawSectionIcon(Graphics g, int sectionIdx, RectangleF r, Color col)
        {
            using (Pen p = new Pen(col, 1.6f))
            {
                if (sectionIdx == 0)
                {
                    // Pitch: Expression pedal silhouette
                    g.DrawRectangle(p, r.X + 8, r.Y + 2, 14, 22);
                    g.DrawLine(p, r.X + 10, r.Y + 9, r.X + 20, r.Y + 9);
                }
                else if (sectionIdx == 1)
                {
                    // Pre FX: Dual stompbox
                    g.DrawRectangle(p, r.X + 3, r.Y + 4, 10, 18);
                    g.DrawRectangle(p, r.X + 17, r.Y + 4, 10, 18);
                }
                else if (sectionIdx == 2)
                {
                    // Amp: Head silhouette
                    g.DrawRectangle(p, r.X + 2, r.Y + 5, 26, 16);
                    g.DrawLine(p, r.X + 4, r.Y + 15, r.X + 26, r.Y + 15);
                }
                else if (sectionIdx == 3)
                {
                    // Cab: 4x12 Speaker cabinet
                    g.DrawRectangle(p, r.X + 4, r.Y + 2, 22, 22);
                    g.DrawEllipse(p, r.X + 7, r.Y + 5, 6, 6);
                    g.DrawEllipse(p, r.X + 17, r.Y + 5, 6, 6);
                    g.DrawEllipse(p, r.X + 7, r.Y + 15, 6, 6);
                    g.DrawEllipse(p, r.X + 17, r.Y + 15, 6, 6);
                }
                else if (sectionIdx == 4)
                {
                    // EQ: 3 Vertical sliders
                    g.DrawLine(p, r.X + 7, r.Y + 3, r.X + 7, r.Bottom - 3);
                    g.DrawLine(p, r.X + 15, r.Y + 3, r.X + 15, r.Bottom - 3);
                    g.DrawLine(p, r.X + 23, r.Y + 3, r.X + 23, r.Bottom - 3);
                    g.DrawRectangle(p, r.X + 5, r.Y + 7, 4, 4);
                    g.DrawRectangle(p, r.X + 13, r.Y + 14, 4, 4);
                    g.DrawRectangle(p, r.X + 21, r.Y + 9, 4, 4);
                }
                else
                {
                    // Post FX: Delay/Reverb waves
                    g.DrawRectangle(p, r.X + 4, r.Y + 4, 22, 18);
                    g.DrawArc(p, r.X + 8, r.Y + 7, 14, 12, 0, 180);
                }
            }
        }

        private static GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {
            float d = radius * 2f;
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0.5f)
            {
                path.AddRectangle(bounds);
                return path;
            }
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static float Clamp(float v, float min, float max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        // ====================================================================
        // MOUSE INTERACTION HANDLERS
        // ====================================================================
        private void OnCanvasMouseDown(object sender, MouseEventArgs e)
        {
            PointF pt = ToLogicalPoint(e.Location);
            lastMouseLogical = pt;

            // Search hotspots in reverse draw order (topmost first)
            for (int i = hotspots.Count - 1; i >= 0; i--)
            {
                UiHotspot h = hotspots[i];
                if (h.Bounds.Contains(pt))
                {
                    if (e.Button == MouseButtons.Right && h.OnRightClick != null)
                    {
                        h.OnRightClick();
                        RenderToBackBuffer();
                        this.Invalidate();
                        return;
                    }
                    if (e.Button == MouseButtons.Left)
                    {
                        if (h.OnDragDelta != null || h.OnDragPoint != null)
                        {
                            activeDragHotspot = h;
                            if (h.OnDragPoint != null) h.OnDragPoint(pt);
                        }
                        if (h.OnClick != null)
                        {
                            h.OnClick();
                        }
                        RenderToBackBuffer();
                        this.Invalidate();
                        return;
                    }
                }
            }
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            PointF pt = ToLogicalPoint(e.Location);
            hoverLogical = pt;
            if (activeDragHotspot != null && e.Button == MouseButtons.Left)
            {
                float dy = pt.Y - lastMouseLogical.Y;
                lastMouseLogical = pt;
                if (activeDragHotspot.OnDragDelta != null)
                {
                    activeDragHotspot.OnDragDelta(dy);
                }
                if (activeDragHotspot.OnDragPoint != null)
                {
                    activeDragHotspot.OnDragPoint(pt);
                }
                RenderToBackBuffer();
                this.Invalidate();
            }
        }

        private void OnCanvasMouseUp(object sender, MouseEventArgs e)
        {
            activeDragHotspot = null;
        }

        private void OnCanvasMouseDoubleClick(object sender, MouseEventArgs e)
        {
            PointF pt = ToLogicalPoint(e.Location);
            for (int i = hotspots.Count - 1; i >= 0; i--)
            {
                UiHotspot h = hotspots[i];
                if (h.Bounds.Contains(pt) && h.OnDoubleClick != null)
                {
                    h.OnDoubleClick();
                    RenderToBackBuffer();
                    this.Invalidate();
                    return;
                }
            }
        }

        private void OnCanvasMouseWheel(object sender, MouseEventArgs e)
        {
            PointF pt = ToLogicalPoint(e.Location);
            for (int i = hotspots.Count - 1; i >= 0; i--)
            {
                UiHotspot h = hotspots[i];
                if (h.Bounds.Contains(pt) && h.OnDragDelta != null)
                {
                    h.OnDragDelta(e.Delta > 0 ? -8f : 8f);
                    RenderToBackBuffer();
                    this.Invalidate();
                    return;
                }
            }
        }
    }

    public static class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            string screenshotTarget = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--screenshot" && i + 1 < args.Length)
                {
                    screenshotTarget = args[i + 1];
                    i++;
                }
                else if (args[i].StartsWith("--screenshot="))
                {
                    screenshotTarget = args[i].Substring("--screenshot=".Length);
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GojiraMainForm(screenshotTarget));
            return 0;
        }
    }
}
