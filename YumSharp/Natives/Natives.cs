using System;
using System.Runtime.InteropServices;

namespace monoe.exe.YumSharp.Natives;

internal static partial class INative
{
#if WINDOWS
    private const string LibName = "yum.dll";
#elif LINUX
    private const string LibName = "libyum.so";
#elif OSX || GODOT_MACOS || GODOT_OSX
  private const string LibName = "libyum.dylib";
#else
    private const string LibName = "yum"; // fallback
#endif

  private const string DllName = $"Libraries/{LibName}";

  public static string SafeIt(IntPtr i) => Marshal.PtrToStringAnsi(i) ?? string.Empty;

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial void YumEngine_init();

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial void YUM_EXPLODE();
}