using System;
using System.Runtime.InteropServices;

namespace monoe.exe.YumSharp.Natives;

internal static partial class INative
{
  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial IntPtr YumEngineInfo_name();

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial IntPtr YumEngineInfo_studioName();

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial IntPtr YumEngineInfo_branch();

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial int YumEngineInfo_versionMajor();

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial int YumEngineInfo_versionMinor();

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial int YumEngineInfo_versionPatch();

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial long YumEngineInfo_longVersion();
}