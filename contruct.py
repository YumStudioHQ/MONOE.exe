import os
import sys
import shutil
import subprocess
from pathlib import Path

class Platform:
    def __init__(self, name, resources, libspath, godot_name, os_name, arch, lib_ext):
        self.name = name
        self.resources = Path(resources)       # Where Godot will see resources
        self.libspath = Path(libspath)         # Where binary/libs live
        self.godot_name = godot_name
        self.os_name = os_name
        self.arch = arch
        self.lib_ext = lib_ext

# Define all target platforms
platforms = [
    Platform("osx",      "build/macOS/monoe.exe.app/Contents/Resources",
                         "build/macOS/monoe.exe.app/Contents/MacOS",
                         "macOS", "apple", None, "dylib"),

    Platform("lin64",    "build/linux/x64",   "build/linux/x64",
                         "Linux", "linux", "x64", "so"),
    Platform("linarm64", "build/linux/arm64", "build/linux/arm64",
                         "Linux 3", "linux", "arm64", "so"),
    Platform("lin32",    "build/linux/x86",   "build/linux/x86",
                         "Linux 2", "linux", "x86", "so"),

    Platform("win64",   "build/windows/x64", "build/windows/x64",
                         "Windows Desktop", "win", "x64", "dll"),
    Platform("winarm64", "build/windows/arm64", "build/windows/arm64",
                         "Windows Desktop 3", "win", "arm64", "dll"),
    Platform("win32",    "build/windows/x86", "build/windows/x86",
                         "Windows Desktop 2", "win", "x86", "dll"),
]

# --------------------------
# Utilities
# --------------------------

def find_godot():
    godot = os.environ.get("GODOT")
    if godot and Path(godot).exists():
        return godot
    godot = shutil.which("godot")
    if godot:
        return godot
    print("Error: Godot executable not found.")
    sys.exit(1)

def copy_libyum(platform: Platform, target_dir: Path):
    """Copy the correct libyum to a given target folder"""
    libs_src = Path("libs")

    if platform.os_name == "apple":
        libname = f"libyum_apple.{platform.lib_ext}"
    else:
        libname = f"libyum_{platform.os_name}_{platform.arch}.{platform.lib_ext}"

    src = libs_src / libname
    dst_dir = target_dir / "libs"
    dst = dst_dir / libname

    if not src.exists():
        print(f"Error: Missing {src}")
        sys.exit(1)

    dst_dir.mkdir(parents=True, exist_ok=True)
    shutil.copy2(src, dst)

# --------------------------
# Build Steps
# --------------------------

def build_runtime(platform: Platform):
    """Step 1: Build runtime Godot project for a platform"""
    print(f"==> Building runtime for {platform.name}")

    result = subprocess.run([
        find_godot(),
        "--headless",
        "--export-release",
        platform.godot_name
    ])
    if result.returncode != 0:
        print(f"Godot runtime export failed for {platform.name}")
        sys.exit(1)

    # Copy the correct libyum to runtime folder
    runtime_lib_dir = Path("runtimes") / platform.name
    copy_libyum(platform, runtime_lib_dir)

def construct(platform: Platform):
    """Step 2: Build main project using the runtime"""
    print(f"==> Constructing main build for {platform.name}")

    result = subprocess.run([
        find_godot(),
        "--headless",
        "--export-release",
        platform.godot_name
    ])
    if result.returncode != 0:
        print(f"Godot export failed for {platform.name}")
        sys.exit(1)

    # Make folders for resources & binary libs
    platform.resources.mkdir(parents=True, exist_ok=True)
    platform.libspath.mkdir(parents=True, exist_ok=True)

    # Copy the correct libyum to binary folder
    copy_libyum(platform, platform.libspath)

    # Copy runtime folder to resources
    runtime_src = Path("runtimes") / platform.name
    if runtime_src.exists():
        shutil.copytree(runtime_src, platform.resources / "runtimes", dirs_exist_ok=True)

    # Copy monoelib
    shutil.copytree(Path("monoelib"), platform.resources / "monoelib", dirs_exist_ok=True)

    # Copy project.lua
    shutil.copy2(Path("main.lua"), platform.resources / "main.lua")

# --------------------------
# Main
# --------------------------

if __name__ == "__main__":
    import bump
    import gendoc

    # Bump version and generate docs
    bump.main()
    gendoc.main()

    # 1️⃣ Build all runtimes first
    for p in platforms:
        build_runtime(p)

    # 2️⃣ Construct main builds using the runtimes
    for p in platforms:
        construct(p)

    print("✅ All builds completed successfully!")
