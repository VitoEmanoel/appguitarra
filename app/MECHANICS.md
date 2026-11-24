# Archetype Gojira X v1.0.2 — Program Mechanics & Decompiled Specification

## 1. Architecture Overview

The target executable `Archetype Gojira X v1.0.2.exe` (`487,293,352` bytes, SHA-256 `d3903e86a701c149ef5a59e0e726e908307147a67b99b44f80816f09b239879d`) is a 32-bit Windows self-extracting installer bootstrapper (`ExternalUi.pdb`, entry point `0x5e0862`) carrying a `483.7 MB` `ADVINSTSFX` overlay (`0x00358000..0x1D0B5780`) that unpacks and installs the 64-bit standalone application `ArchetypeGojiraX.exe` (`129,969,992` bytes, SHA-256 `d8c80a371db8b9bd084b6437720a0f1c2c5bb602a8cb5fbfd33364188c9c9841`), VST2/VST3/AAX plugins, PDF manual (`ArchetypeGojiraXv1.0.0.pdf`), and `321` factory preset XML files.

---

## 2. Installer Bootstrapper & Payload Extraction (`Archetype Gojira X v1.0.2.exe`)

| Item | Value | Source Address / Function | Notes |
|---|---|---|---|
| Bootstrapper Entry Point | `0x5e0862` | `function 0x5e0862` | MSVC x86 GUI entrypoint |
| General Options Parser | `[GeneralOptions]` | `function 0x558830` | Reads `ProductVersion`, `ExtractionFolder`, `FileExtractFirst`, `DefaultProdCode`, `UpgradeCode`, `PackageCode` |
| Install Mode Complete | `2` (`L"complete"`) | `function 0x558830` | `*(uint32_t *)(this + 0x28) = 2` |
| Install Mode Minimum | `1` (`L"minimum"`) | `function 0x558830` | `*(uint32_t *)(this + 0x28) = 1` |
| Install Mode Prereqs | `5` (`L"prereqs"`) | `function 0x558830` | `*(uint32_t *)(this + 0x28) = 5` |
| SFX Footer Table Reader | `ADVINSTSFX` | `function 0x5557b0`, `function 0x539d60` | Reads footer at file offset `0x1D0B5738` (`487,282,488`) before Authenticode table (`0x1D0B5780`) |
| Embedded Stream Extractor | `0x10000` (`65536` bytes) | `function 0x550ce0` | Seeks to `base_offset + entry_offset`, reads in `64 KiB` blocks, calls `0x5709d0` when `(flags >> 1) & 1 != 0` |
| Header Bitwise Inverter | `~byte` (`XOR 0xFF`, `512` bytes) | `function 0x5709d0` | Inverts first `512` bytes (`0x200`) of obfuscated MSI/CAB streams using SIMD mask at `0x69cf80` |
| Cabinet FDI Unpacker | `FDICreate` / `FDICopy` | `function 0x413640` | Unpacks `Archetype Gojira X1.cab` (`328` files, `1` folder) via `Cabinet.dll` |
| Embedded MSI Offset & Size | `0x00358000` (`2,083,840` bytes) | `function 0x550ce0` | `Archetype Gojira X.msi` |
| Embedded CAB Offset & Size | `0x00554C00` (`481,691,684` bytes) | `function 0x550ce0` | `Archetype Gojira X1.cab` (`328` files including `ArchetypeGojiraX.exe` and `321` presets) |
| Embedded INI Offset & Size | `0x1D0B5424` (`788` bytes) | `function 0x558830` | `Archetype Gojira X v1.0.2 RC1.ini` (`FileExtractFirst=1`) |

---

## 3. Window, Scaling & Theme Constants (`ArchetypeGojiraX.exe` `.rdata`)

| Constant | Value | Source Address | Notes |
|---|---|---|---|
| Default Window Width | `1024` px | `address 0x146562e30` (`configuration.xml`) | `<window width="1024" height="782"/>` |
| Default Window Height | `782` px | `address 0x146562e30` (`configuration.xml`) | `750` px main canvas + `32` px bottom utility bar |
| Main Section Canvas Height | `750` px | `address 0x1465a3970` (`applicationView.xml`) | `sectionsContainerView` bounds `w:100%, h:750px` |
| Top Bar Height | `190` px | `address 0x1465a3970` (`applicationView.xml`) | `topBarView` bounds `w:100%, h:190px` |
| Bottom Utility Bar Height | `32` px | `address 0x1465a3970` (`applicationView.xml`) | `bottomBarView` bounds `y:750px, w:100%, h:32px` |
| Minimum Scale Factor | `0.5` | `address 0x146562e30` (`configuration.xml`) | `minScaleFactor="0.5"` |
| Maximum Scale Factor | `2.0` | `address 0x146562e30` (`configuration.xml`) | `maxScaleFactor="2.0"` |
| Scale Small | `0.715` | `address 0x146562e30` (`configuration.xml`) | `<scale scaleName="Small" factor="0.715"/>` |
| Scale Medium | `1.0` | `address 0x146562e30` (`configuration.xml`) | `<scale scaleName="Medium" factor="1.0"/>` |
| Scale Large | `1.3` | `address 0x146562e30` (`configuration.xml`) | `<scale scaleName="Large" factor="1.3"/>` |
| Scale X-Large | `1.6` | `address 0x146562e30` (`configuration.xml`) | `<scale scaleName="X-Large" factor="1.6"/>` |
| Scale Maximum | `2.0` | `address 0x146562e30` (`configuration.xml`) | `<scale scaleName="Maximum" factor="2.0"/>` |
| Double-Click Reset Timeout | `200` ms | `address 0x146562e30` (`configuration.xml`) | `<mouse doubleClickTimeout="200"/>` |
| Background Colour | `#161616` | `address 0x1465a5c40` (`styles.xml`) | `backgroundColour="FF161616"` |
| Delay LCD Text Colour | `#BDC6C0` | `address 0x1465a5c40` (`styles.xml`) | `fxScreenTextColour="FFBDC6C0"` |
| Delay LCD Editable Colour | `#E47A17` | `address 0x1465a5c40` (`styles.xml`) | `fxScreenEditableTextColour="FFE47A17"` |
| Tuner In-Tune / Meter Green | `#45F862` | `address 0x14671ab90` (`archetypeStyles.xml`) | `success="FF45F862"` |
| Clip / Out-of-Tune Red | `#FF2727` | `address 0x14671ab90` (`archetypeStyles.xml`) | `errors="FFFF2727"` |

---

## 4. Global Audio Controls, Sections & Navigation (`0x14658cb30`, `0x14658fa90`)

- **6 Section Modules** (`selectedSection` at `address 0x14658cb30`, default `2` = `Amp`):
  - `0`: `Pitch` (`WOW` Pitch Shifter + `OCT` Octaver)
  - `1`: `Pedal` (`Pre FX`: `OD` Overdrive + `DRT` Distortion + `PHSR` Phaser + `CHR` Chorus)
  - `2`: `Amp` (`CLN` Clean, `RST` Rust/Crunch, `HOT` Lead)
  - `3`: `Cab` (Dual Microphone IR Cabinet Simulator)
  - `4`: `EQ` (3x 9-Band Graphic Equalizers + HPF/LPF)
  - `5`: `FX` (`Post FX`: `DLY` Delay + `REV` Reverb)
  - Right-clicking or double-clicking any section icon toggles its `sectionActive` bypass (`default = true`).
- **Amp & Cab Selection** (`address 0x14658cb30`):
  - `selectedAmp`: `0 = CLN`, `1 = RST`, `2 = HOT` (default `2 = HOT`)
  - `selectedCab`: `0 = CLN`, `1 = RST`, `2 = HOT` (default `0 = CLN`, or follows `selectedAmp` when linked)
  - `ampCabLinkedState`: boolean (UI default linked in standalone session; toggling unlinks amp head from cabinet selection; switching Amp always switches the active 9-Band Graphic EQ)
- **Top Bar Parameters** (`address 0x14658fa90`, `parametersModel.xml`):
  - `inputGain`: `-24.0` to `+24.0` dB, default `0.0` dB
  - `outputGain`: `-24.0` to `+24.0` dB, default `0.0` dB
  - `gateActive`: boolean, default `true`
  - `gateThreshold`: `-96.0` to `0.0` dB, default `-80.0` dB (Kone node normalized `0.1` at `0x142351920`)
  - `transpose`: integer `-12` to `+12` semitones, default `0` st (bypassed at `0`)
  - `selectedMode` (Input Mode): `0 = Mono`, `1 = Stereo`, default `0` (`Mono`)
  - `doublerActive`: boolean, default `false` (disabled when Input Mode is `Stereo`)
  - `doublerSpread`: `3.0` to `20.0` ms, default `6.0` ms
  - `tapTempoBpm`: `40.0` to `240.0` BPM, default `120.0` BPM
  - `inputMeter` / `outputMeter` (`address 0x146716c70`): `-60.0` to `0.0` dB, release `500.0` ms (`0x142351920`), setpoint `0.775` (`0x1465ae530`), red clip indicator cleared on click.

---

## 5. Signal Chain & Per-Section Parameters (`0x14234a410..0x146590640`)

### 5.1 Pitch Section (`0x14658c1d0`, `0x14658c510`, `0x146590640`)
- **`WOW` — Pitch Shifter Pedal** (`whammyPedalParametersModel.xml` at `0x14658c510`):
  - `whammyActive`: default `false`
  - `whammyPedal` (Expression Heel-to-Toe): `0.0` to `1.0`, default `0.5` (Kone default `0.0`)
  - `whammyMix` (`FATSO MIX`): `0.0` to `1.0`, default `0.5` (Kone default `1.0`; disabled in `Blade 1` and `Blade 2` modes)
  - `whammyMode`:
    - `0` = `Fatso`: `-12` semitones (heel `0.0`) to `+12` semitones (toe `1.0`)
    - `1` = `Blade 1`: `0` semitones (heel `0.0`) to `+12` semitones (toe `1.0`)
    - `2` = `Blade 2`: `0` semitones (heel `0.0`) to `+24` semitones (toe `1.0`)
- **`OCT` — Octaver Pedal** (`octaverPedalParametersModel.xml` at `0x14658c1d0`):
  - `octaverActive`: default `false`
  - `octaverOct1` (`-12` semitones layer): `0.0` to `1.0`, default `0.5`
  - `octaverOct2` (`-24` semitones layer): `0.0` to `1.0`, default `0.5`
  - `octaverLevel` (`DIRECT LEVEL`): `0.0` to `1.0`, default `0.5` (Kone default `1.0`)

### 5.2 Pre FX Section (`0x14658b580..0x14658bfd0`, `0x1465903d0`)
Four pedals in series (`OD` -> `DRT` -> `PHSR` -> `CHR`):
- **`OD` — Overdrive Pedal** (`0x14658bca0`):
  - `odActive`: default `false`
  - `odDist`, `odLevel`, `odTone`: `0.0` to `1.0`, default `0.5`
- **`DRT` — Distortion Pedal** (`0x14658b970`):
  - `drtActive`: default `false`
  - `drtDist`, `drtLevel` (`VOL`), `drtTone` (`FILTER` low-pass): `0.0` to `1.0`, default `0.5`
- **`PHSR` — Phaser Pedal** (`0x14658bfd0`):
  - `phsrActive`: default `false`
  - `phsrRate`: `0.12` to `10.0` Hz, default `0.614` Hz, skew `0.205`
- **`CHR` — Chorus Pedal** (`0x14658b580`):
  - `chrActive`: default `false`
  - `chrRate`: `0.12` to `10.0` Hz, default `0.41` Hz, skew `0.4`
  - `chrDepth`, `chrFeedback`, `chrMix`: `0.0` to `1.0`, default `0.5`

### 5.3 Amp Section (`0x146587810..0x1465882c0`, `0x14658c900`)
- **`CLN` — Clean Amplifier** (`0x146587810`):
  - `cleanAmpBright`: boolean switch, default `false`
  - `cleanAmpGain`, `cleanAmpBass`, `cleanAmpMid`, `cleanAmpTreble`, `cleanAmpLevel`: `0.0` to `1.0`, default `0.5` (`cleanAmpLevel` maps linearly to `-12.0..+12.0` dB per `0x146563d70`)
- **`RST` — Rust / Crunch Amplifier** (`0x1465882c0`):
  - `rhythmAmpGain`, `rhythmAmpLow` (`BASS`), `rhythmAmpMid`, `rhythmAmpHigh` (`TREBLE`), `rhythmAmpMaster`, `rhythmAmpPresence`, `rhythmAmpDepth`, `rhythmAmpLevel`: `0.0` to `1.0`, default `0.5` (`rhythmAmpLevel` maps to `-12.0..+12.0` dB)
- **`HOT` — Lead Amplifier** (`0x146587ca0`):
  - `leadAmpGain`, `leadAmpLow` (`BASS`), `leadAmpMid`, `leadAmpHigh` (`TREBLE`), `leadAmpMaster`, `leadAmpPresence`, `leadAmpDepth`, `leadAmpLevel`: `0.0` to `1.0`, default `0.5` (`leadAmpLevel` maps to `-12.0..+12.0` dB)

### 5.4 Cab Section (`0x14658d670`)
Dual IR Loader (`Left` & `Right` virtual microphones / custom IR slots):
- `leftCabActive`: default `true`; `rightCabActive`: default `true`
- `leftCabPosition`, `rightCabPosition`: `0.000` to `1.000`, default `0.300`
- `leftCabDistance`, `rightCabDistance`: `0.000` to `1.000`, default `0.000`
- `leftCabMicLevel`, `rightCabMicLevel`: `-40.0` to `+6.0` dB, default `0.0` dB, skew `4.9595`
- `leftCabPan`, `rightCabPan`: `-50.0` (`L50`) to `+50.0` (`R50`), default `0.0` (`C`)
- `leftCabPhase`, `rightCabPhase`: boolean phase invert, default `false`
- Factory Microphone Models per Cabinet:
  - Cab 0 (`CLN`) & Cab 1 (`RST`): `0=Dynamic 57`, `1=Dynamic 421`, `2=Condenser 184`, `3=Condenser 414`, `4=Ribbon 121`, `5=Ribbon 160`, `6=Custom IR` (Left default `0`, Right default `4`)
  - Cab 2 (`HOT`): `0=Dynamic 57`, `1=Dynamic 57 Off-Axis`, `2=Dynamic 421`, `3=Condenser 184`, `4=Condenser 414`, `5=Ribbon 121`, `6=Ribbon 160`, `7=Custom IR` (Left default `0`, Right default `4`)
- Visual speaker cone dragging updates `(position, distance)` directly (`closeMicScaler = 0.55`, `farMicScaler = 0.65` at `0x1465a5c40`).

### 5.5 EQ Section (`0x1465888f0..0x146589a90`, `0x146597f40`)
Three independent 9-Band Graphic EQs (`cleanEQ`, `rhythmEQ`, `leadEQ`) switched with the active Amp:
- `9` Frequency Bands (`address 0x146597f40`): `65 Hz`, `125 Hz`, `250 Hz`, `500 Hz`, `1 kHz`, `2 kHz`, `4 kHz`, `8 kHz`, `16 kHz`
- Band Gain Range: `-12.0` to `+12.0` dB, default `0.0` dB
- High-Pass Filter (`HPF`): `20.0` to `500.0` Hz, default `20.0` Hz (`OFF`), skew `0.6`
- Low-Pass Filter (`LPF`): `1000.0` to `20000.0` Hz, default `20000.0` Hz (`OFF`), skew `0.6`

### 5.6 Post FX Section (`0x14658a370`, `0x14658b150`)
- **`DLY` — Delay Pedal** (`0x14658a370`):
  - `delayActive`: default `false`
  - `delayMix`, `delayFeedback`, `delayTone`, `delayTapeSat`, `delayTapeMod`: `0.0` to `1.0`, default `0.5`
  - `delayTime`: `7.0` to `3000.0` ms, default `400.0` ms, skew `0.6`
  - `delayTempo`: `40.0` to `240.0` BPM, default `120.0` BPM
  - `delaySync`: `0 = Free`, `1 = DAW/APP`, `2 = Tap`, default `1`
  - `delaySyncNote`: `21` subdivisions (`0=1/64T` .. `13=1/4` .. `20=1/1D`), default `13` (`1/4`)
  - `delayPingPong`: boolean, default `true`
- **`REV` — Reverb Pedal** (`0x14658b150`, `0x146563d70`):
  - `reverbActive`: default `false`
  - `reverbMix`: `0.0` to `1.0`, default `0.5`
  - `reverbDecay`: `0.0` to `1.0`, default `0.5` (maps to `250.0..10000.0` ms)
  - `reverbLowCut` (`HIGH PASS FILTER`): `0.0` to `1.0`, default `0.0` (maps to `50.0..700.0` Hz)
  - `reverbHighCut` (`LOW PASS FILTER`): `0.0` to `1.0`, default `1.0` (maps to `1000.0..10000.0` Hz)
  - `reverbShimmer`: boolean, default `false`

---

## 6. Utility Bar: Tuner, Metronome, Presets, MIDI & Audio Settings

- **Chromatic Tuner** (`address 0x1467195b0`, `0x146745080`):
  - `tunerReferenceFrequency`: `420.0` to `460.0` Hz, default `440.0` Hz (manual notes `400-480 Hz` selector range)
  - `tunerDifferenceInCents`: `-100.0` to `+100.0` cents (display ticks `-50` to `+50`)
  - `tunerMuteActive`: boolean mute DI monitoring
  - `showHertzValue`: toggles `Cents` vs `Hz`
  - `liveTunerVisible`: toggles compact Live Tuner in bottom utility bar (`Ctrl+Click` on TUNER button)
- **Metronome** (`address 0x146717260`, `0x146738f60`):
  - `metronomeVolume`: `-12.0` to `+12.0` dB, default `0.0` dB
  - `metronomePan`: `-50` to `+50`, default `0` (`C`)
  - `metronomeTimeSignature`: `21` signatures (`2/4`, `3/4`, `4/4` [default `2`], `5/4`, `6/4`, `7/4`, `8/4`, `9/4`, `10/4`, `11/4`, `12/4`, `13/4`, `3/8`, `6/8`, `9/8`, `12/8`, `5/8 (3+2)`, `5/8 (2+3)`, `7/8 (3+2+2)`, `7/8 (2+3+2)`, `7/8 (2+2+3)`)
  - `metronomeSoundSet`: `6` sets (`Blip`, `Block and Shaker`, `Cowbell`, `Digital`, `Drum Kit`, `Soft Kit`)
  - `metronomeNoteLength` (Rhythm subdivisions): `0..3`
  - `metronomeStep1State..metronomeStep13State`: up to `13` toggleable beat accent LEDs (`0=off, 1=weak, 2=strong, 3=sub`, `step1` default `2`)
- **Preset Manager** (`address 0x146563d70`, `0x14673fa50`):
  - `4` Categories: `Gojira`, `Artists`, `Neural DSP`, `User` (`321` factory presets unpacked from `Archetype Gojira X1.cab`)
  - Controls: Previous/Next arrows, Preset dropdown browser, `DELETE`, `SAVE`, `SAVE AS...`, contextual menu (`IMPORT`, `RESET`, `LOCATE FILE`), dirty asterisk `*` when modified.
- **MIDI Mappings** (`address 0x1467185f0`, `0x146718ad0`):
  - Supports `CC Absolute`, `CC Relative`, `CC Toggle`, `CC Dec+Inc`, `Note Toggle`, `PC Preset`, etc. across `16` channels (`1..16`), `Data1/Data2` (`0..127`), right-click `Enable MIDI Learn` highlights target parameter in green (`#45F862`).
- **Estimated Clean-Room DSP & Encrypted Preset Values** (`estimated: true`):
  - Because `ArchetypeGojiraX.exe`'s `.text` code section is PACE/iLok-wrapped (entropy `8.00` bits/byte) and factory preset XML payloads are encrypted on disk, internal neural network weights, convolution IR audio samples, and factory preset parameter vectors are estimated using clean-room DSP transfer curves (cascaded tanh tube saturation, biquad tone stack and 9-band graphic EQ filters, LFO phaser/chorus, delay line with tape saturation/modulation, shimmer reverb, pitch shifter/octaver) and curated preset parameter sets matching the extracted factory preset names.
