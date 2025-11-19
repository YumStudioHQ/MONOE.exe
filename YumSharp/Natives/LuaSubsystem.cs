using System;
using System.Runtime.InteropServices;

namespace monoe.exe.YumSharp.Natives;

internal static partial class INative
{
  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial int YumLuaSubsystem_load(
      IntPtr subsystem, ulong uid,
      [MarshalAs(UnmanagedType.LPStr)] string src,
      [MarshalAs(UnmanagedType.I1)] bool isFile);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  [return: MarshalAs(UnmanagedType.I1)]
  public static partial bool YumLuaSubsystem_good(IntPtr subsystem, ulong uid);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial IntPtr YumLuaSubsystem_call(
      IntPtr subsystem, ulong uid,
      [MarshalAs(UnmanagedType.LPStr)] string name,
      IntPtr args);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void YumCallback(IntPtr inVec, IntPtr outVec);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial int YumLuaSubsystem_pushCallback(
      IntPtr subsystem,
      ulong uid,
      [MarshalAs(UnmanagedType.LPStr)] string name,
      YumCallback cb,
      [MarshalAs(UnmanagedType.LPStr)] string ns);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial int YumLuaSubsystem_hasMethod(IntPtr s, ulong uid, [MarshalAs(UnmanagedType.LPStr)] string path);

}