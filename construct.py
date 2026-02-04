# Please know that this file is pure ugly / AI gen code ...
# "But it works on my computer" — LOL.

import os
import shutil
import subprocess
import sys
from pathlib import Path
from typing import List, Tuple
import time

class C:
    RESET = "\033[0m"
    DIM = "\033[2m"
    BOLD = "\033[1m"

    RED = "\033[31m"
    GREEN = "\033[32m"
    YELLOW = "\033[33m"
    BLUE = "\033[34m"
    GRAY = "\033[90m"


class log:
    @staticmethod
    def info(msg: str) -> None:
        print(f"{C.GRAY}• {msg}{C.RESET}")

    @staticmethod
    def ok(msg: str) -> None:
        print(f"{C.GREEN}✓ {msg}{C.RESET}")

    @staticmethod
    def warn(msg: str) -> None:
        print(f"{C.YELLOW}! {msg}{C.RESET}")

    @staticmethod
    def err(msg: str) -> None:
        print(f"{C.RED}✗ {msg}{C.RESET}")

    @staticmethod
    def header(msg: str) -> None:
        print(f"\n{C.BOLD}{msg}{C.RESET}")
        print(f"{C.DIM}{'─' * len(msg)}{C.RESET}")

platforms: List[Tuple[str, str, str, str, str]] = [
    ("macOS", "build/osx/monoe.exe.app/Contents/MacOS", "libyum_apple.dylib",
        "build/osx/monoe.exe.app/Contents/Resources", 'APPLE'),
    ("Linux", "build/lin64", "libyum_linux_x64.so", "build/lin64", 'x86_64-linux-gnu'),
    ("Linux 2", "build/lin32", "libyum_linux_x86.so", "build/lin32", 'x86-linux-gnu'),
    ("Linux 3", "build/linarm64", "libyum_linux_arm64.so", "build/linarm64", 'aarch64-linux-gnu'),
    ("Windows Desktop", "build/win64", "libyum_win_x64.dll", "build/win64", 'x86_64-windows-gnu'),
    ("Windows Desktop 2", "build/win32", "libyum_win_x86.dll", "build/win32", 'x86-windows-gnu'),
    ("Windows Desktop 3", "build/winarm64", "libyum_win_arm64.dll", "build/winarm64", 'aarch64-windows-gnu'),
]

def find_godot() -> str:
    godot = os.environ.get("GODOT")
    if godot and Path(godot).exists():
        return godot

    godot = shutil.which("godot")
    if godot:
        return godot

    log.err("Godot executable not found (set GODOT or add to PATH)")
    sys.exit(1)


def build(platform_name: str) -> None:
    result = subprocess.run(
        [find_godot(), "--headless", "--export-release", platform_name],
        stdout=subprocess.DEVNULL,
        stderr=sys.stderr
    )
    if result.returncode != 0:
        log.err(f"Godot export failed for {platform_name}")
        sys.exit(1)


def clean_folder(folder: Path) -> None:
    if folder.exists():
        for item in folder.iterdir():
            if item.is_dir():
                shutil.rmtree(item)
            else:
                item.unlink()


def zip_folder(folder: Path, output_name: Path) -> None:
    shutil.make_archive(str(output_name), "zip", folder)
    log.ok(f"Created {output_name.name}.zip")


def codesign_app(app_path: Path) -> None:
    codesign_dir = app_path / "Contents/_CodeSignature"
    if codesign_dir.exists():
        shutil.rmtree(codesign_dir)
        log.info("Removed old code signature")

    result = subprocess.run(
        ["codesign", "--force", "--deep", "--sign", "-", str(app_path)],
        capture_output=True,
        text=True
    )

    if result.returncode != 0:
        log.err("Codesign failed")
        print(result.stderr)
    else:
        log.ok("App re-signed (ad-hoc)")


def zip_all_build_folders(build_dir: Path) -> None:
    log.header("Final Build Archives")
    for item in build_dir.iterdir():
        if item.is_dir():
            zip_path = build_dir / item.name
            shutil.make_archive(str(zip_path), "zip", item)
            log.ok(f"{item.name}.zip created")

def cxx(out: str, apple: bool, target: str):
    result = None
    if apple:
        ret = os.system(f'clang++ -arch arm64 -arch x86_64 -stdlib=libc++ -isysroot $(xcrun --show-sdk-path) -Wall -Wextra main/main.cpp -o "{out}" -std=c++23')
        if ret != 0: sys.exit(ret)
        else: return
    else:
        result = subprocess.run(
            ['zig', 'c++', 'main/main.cpp', '-target', target, '-o', out, '-Wall', '-Wextra', '-std=c++23'],
            stdout=subprocess.DEVNULL,
            stderr=sys.stderr
        )
    if result.returncode != 0:
        log.err(f"Failed to compile C++ for platform {target}")
        sys.exit(1)

def install_bootloader(contents: Path, binpath: Path, platform: str):
    if platform == 'APPLE':
        cxx(str(binpath / 'launch'), True, platform)
        old = "<key>CFBundleExecutable</key>\n\t<string>monoe.exe</string>"
        new = "<key>CFBundleExecutable</key>\n\t<string>launch</string>"
        with open(contents.parent / 'Info.plist', 'r') as file:
            content = file.read().replace(' ', '').replace('\t', '').replace(old, new)
            file.close()
            with open(contents.parent / 'Info.plist', 'w') as wfile: wfile.write(content); wfile.close()
    else:
        cxx(str(binpath / 'launch'), False, platform)
    log.ok('C++ entry built');

def main() -> None:
    build_dir = Path("build")
    runtimes_dir = Path("runtimes")

    log.header("Preparing Runtimes")

    if runtimes_dir.exists():
        shutil.rmtree(runtimes_dir)
        log.info("Removed old runtimes/")

    runtimes_dir.mkdir(exist_ok=True)

    for platform_name, binpath, libyumsrc, resource_path, cxx_plat in platforms:
        log.header(platform_name)

        bin_path_obj = Path(binpath)
        clean_folder(bin_path_obj)
        log.info("Cleaned build folder")

        build(platform_name)
        log.ok("Godot export completed")

        # Copy libyum
        libyum_dir = bin_path_obj / "libs"
        libyum_dir.mkdir(exist_ok=True)

        src = Path("libs") / libyumsrc
        dest = libyum_dir / f"libyum{src.suffix}"
        shutil.copy(src, dest)
        log.ok(f"{dest.name} installed")

        # Copy monoelib and main.lua
        resource_dir = Path(resource_path)
        monoelib_target = resource_dir / "monoelib"

        if monoelib_target.exists():
            shutil.rmtree(monoelib_target)

        shutil.copytree("monoelib", monoelib_target)
        shutil.copy2("main.lua", resource_dir)
        log.ok("monoelib/ and main.lua copied")

        install_bootloader(Path(resource_path), Path(binpath), cxx_plat)

        # Zip runtime
        platform_build_dir = (
            bin_path_obj.parent.parent
            if platform_name == "macOS"
            else bin_path_obj
        )

        tname = "osx" if platform_name == "macOS" else binpath.replace("build/", "")
        zip_folder(platform_build_dir, runtimes_dir / tname)

        if platform_name == "macOS":
            codesign_app(bin_path_obj.parent.parent)

    log.header("Embedding Runtimes")

    for _, _, _, resource_path, _ in platforms:
        resource_dir = Path(resource_path)
        target = resource_dir / "runtimes"

        if target.exists():
            shutil.rmtree(target)

        shutil.copytree(runtimes_dir, target)
        log.ok(f"runtimes → {resource_dir}")

    log.header('signing again (with runtimes now)')
    codesign_app(Path('build/osx/monoe.exe.app'))

    zip_all_build_folders(build_dir)

if __name__ == "__main__":
    import bump 
    import gendoc

    start = time.time()

    log.header("Build Started")

    bump.main()
    main()
    gendoc.main()

    elapsed = time.time() - start
    log.header("Done")
    log.ok(f"Total time: {elapsed / 60:.2f} minutes")
