using System;
using System.Runtime.InteropServices;

namespace monoe.exe.YumSharp.Natives;

internal static partial class INative
{
  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial void Yum_open_G_out([MarshalAs(UnmanagedType.LPStr)] string path);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial void Yum_open_G_err([MarshalAs(UnmanagedType.LPStr)] string path);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial void Yum_open_G_in([MarshalAs(UnmanagedType.LPStr)] string path);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void YumRedirectionCallback([MarshalAs(UnmanagedType.LPStr)] string str);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial void Yum_redirect_G_out(YumRedirectionCallback callback);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial void Yum_redirect_G_err(YumRedirectionCallback callback);
}