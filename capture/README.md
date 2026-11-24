# What we found in C:\Users\Ve778\Downloads\Archetype Gojira X v1.0.2.exe

Archetype Gojira X v1.0.2.exe is a program for Windows, built for x86 (32-bit). Its internal names were removed (stripped), so most functions will have generated names.

Evidence ev_3b7036faaac2a8c27b7a587f1ec90d25a3a5f9281f6b97c5365ab8054a848770.

```json
{
  "architecture": "x86",
  "bits": 32,
  "dotnet_arch": null,
  "entry_point": "0x5e0862",
  "format": "pe",
  "format_name": "PE (Windows)",
  "kind": "executable",
  "libraries": [
    "KERNEL32.dll",
    "msi.dll",
    "gdiplus.dll",
    "Cabinet.dll",
    "USER32.dll",
    "GDI32.dll",
    "ADVAPI32.dll",
    "SHELL32.dll",
    "ole32.dll",
    "OLEAUT32.dll",
    "SHLWAPI.dll",
    "COMCTL32.dll",
    "UxTheme.dll",
    "MSIMG32.dll",
    "dbghelp.dll",
    "VERSION.dll",
    "WININET.dll",
    "dwmapi.dll",
    "MPR.dll",
    "NETAPI32.dll"
  ],
  "next_steps": [
    {
      "tool": "core_overview_show",
      "why": "a one-page summary: sections, imports, counts"
    },
    {
      "tool": "native_functions_list",
      "why": "every function we found"
    },
    {
      "tool": "native_strings_find",
      "why": "find text the program shows or uses"
    },
    {
      "tool": "native_function_study",
      "why": "a full dossier on one function (start with main or the entry point)"
    },
    {
      "tool": "core_feature_find",
      "why": "follow a word you see in the app to the code behind it"
    },
    {
      "tool": "native_pe_resources_list",
      "why": "icons, version information and the manifest"
    }
  ],
  "operating_system": "Windows",
  "plain_english": "Archetype Gojira X v1.0.2.exe is a program for Windows, built for x86 (32-bit). Its internal names were removed (stripped), so most functions will have generated names.",
  "recommended_toolsets": [
    "core",
    "native",
    "formats"
  ],
  "stripped": true,
  "symbol_table_stripped": true,
  "toolchain_hints": [
    {
      "category": "compiler",
      "evidence": "Rich header",
      "id": "msvc",
      "label": "Microsoft Visual C++ toolchain",
      "strength": "strong"
    }
  ]
}
```

## Ghidra

Deep mode: Ghidra has analysed C:\Users\Ve778\Downloads\Archetype Gojira X v1.0.2.exe (1 min), so decompiling it is quick.
