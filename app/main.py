import os
import subprocess
import sys
import tempfile
from pathlib import Path


def build_if_needed(app_dir: Path, exe_path: Path) -> None:
    sources = sorted(app_dir.glob("*.cs"))
    source_modified_at = max((source.stat().st_mtime_ns for source in sources), default=0)
    if exe_path.is_file() and exe_path.stat().st_mtime_ns >= source_modified_at:
        return

    windows_dir = os.environ.get("SystemRoot")
    if not windows_dir or not sources:
        raise RuntimeError("A compilação requer o Windows e os arquivos C# na pasta app.")
    framework_dir = Path(windows_dir) / "Microsoft.NET"
    compilers = (
        framework_dir / framework / "v4.0.30319" / "csc.exe"
        for framework in ("Framework64", "Framework")
    )
    compiler = next((path for path in compilers if path.is_file()), None)
    if compiler is None:
        raise RuntimeError("O compilador C# do .NET Framework não foi encontrado.")

    with tempfile.TemporaryDirectory(dir=app_dir) as build_dir:
        build_path = Path(build_dir) / exe_path.name
        subprocess.run(
            [
                str(compiler), "/nologo", "/target:winexe", "/optimize+",
                "/platform:anycpu", "/codepage:65001", "/reference:System.Drawing.dll",
                "/reference:System.Windows.Forms.dll", f"/out:{build_path}",
                *(str(source) for source in sources),
            ],
            cwd=app_dir,
            check=True,
        )
        try:
            build_path.replace(exe_path)
        except PermissionError:
            if not exe_path.is_file():
                raise
            if exe_path.stat().st_mtime_ns < source_modified_at:
                raise RuntimeError("Feche o appguitarra antes de atualizar o executável.") from None


def main() -> int:
    app_dir = Path(__file__).resolve().parent
    exe_path = app_dir / "appguitarra.exe"
    try:
        build_if_needed(app_dir, exe_path)
        return subprocess.run([str(exe_path), *sys.argv[1:]], cwd=app_dir).returncode
    except (OSError, RuntimeError, subprocess.CalledProcessError) as error:
        print(error, file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
