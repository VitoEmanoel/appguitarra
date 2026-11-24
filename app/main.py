import os
import subprocess
import sys


def main() -> int:
    app_dir = os.path.dirname(os.path.abspath(__file__))
    exe_path = os.path.join(app_dir, "ArchetypeGojiraX.exe")
    cmd = [exe_path] + sys.argv[1:]
    result = subprocess.run(cmd, cwd=app_dir)
    return int(result.returncode)


if __name__ == "__main__":
    raise SystemExit(main())
