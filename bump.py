#!/usr/bin/env python3

import sys
import re
import subprocess
from pathlib import Path

PROJECT_FILE = Path("project.godot")
VERSION_FILE = Path("Core/Version.cs")
DEFAULT_BUMP = "fix"

VERSION_REGEX = re.compile(
    r'config/version\s*=\s*"(\d+)\.(\d+)\.(\d+)"'
)

SEMVER_REGEX = re.compile(r"^\d+\.\d+\.\d+$")

def git_info():
    try:
        commit = subprocess.check_output(
            ["git", "rev-parse", "HEAD^{tree}",]
        ).decode().strip()
        dirty = bool(
            subprocess.check_output(["git", "status", "--porcelain"]).decode().strip()
        )
        return commit, dirty
    except Exception:
        return "unknown", True

def bump_version(major: int, minor: int, fix:int, bump_type: str) -> tuple[int, int, int]:
    if bump_type == "major":
        return major + 1, 0, 0
    elif bump_type == "minor":
        return major, minor + 1, 0
    else:  # fix / patch
        return major, minor, fix + 1

def get_version() -> str:
    if not PROJECT_FILE.exists():
        raise FileNotFoundError("project.godot not found")

    content = PROJECT_FILE.read_text(encoding="utf-8")
    match = VERSION_REGEX.search(content)

    if not match:
        raise ValueError("Version not found in project.godot")

    major, minor, fix = map(int, match.groups())
    return f"{major}.{minor}.{fix}"

def main():
    args = sys.argv[1:]

    set_version = None
    bump_type = DEFAULT_BUMP

    if args:
        if args[0] == "--set":
            if len(args) != 2 or not SEMVER_REGEX.match(args[1]):
                print("Usage: bump_version.py --set X.Y.Z")
                sys.exit(1)
            set_version = args[1]
        else:
            bump_type = args[0].lower()
            if bump_type == "patch":
                bump_type = "fix"
            if bump_type not in {"major", "minor", "fix"}:
                print("Usage: bump_version.py [major|minor|fix]")
                sys.exit(1)

    if not PROJECT_FILE.exists():
        print("Error: project.godot not found")
        sys.exit(1)

    content = PROJECT_FILE.read_text(encoding="utf-8")
    match = VERSION_REGEX.search(content)

    if not match:
        print("Error: version not found in project.godot")
        sys.exit(1)

    old_major, old_minor, old_fix = map(int, match.groups())
    old_version = f"{old_major}.{old_minor}.{old_fix}"

    if set_version:
        new_version = set_version
    else:
        new_major, new_minor, new_fix = bump_version(
            old_major, old_minor, old_fix, bump_type
        )
        new_version = f"{new_major}.{new_minor}.{new_fix}"

    updated_content = VERSION_REGEX.sub(
      f'config/version="{new_version}"',
      content
    )
    
    commit, dirty = git_info()

    PROJECT_FILE.write_text(updated_content, encoding="utf-8")

    VERSION_FILE.write_text(f"""namespace monoe.exe.Core;
public static partial class Version {{
    public static readonly int Major = {new_major};
    public static readonly int Minor = {new_minor};
    public static readonly int Fix = {new_fix};
    public static readonly string All = "{new_version}.{'dev' if dirty else 'stable'}.{commit}";
    public static readonly string GitCommit = "{commit}";
    public static readonly bool IsDirty = {str(dirty).lower()};
}}""", encoding="utf-8")

    print(f"Version updated: {old_version} → {new_version}")

if __name__ == "__main__":
    main()
