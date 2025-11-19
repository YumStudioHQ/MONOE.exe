using System;
using System.Runtime.InteropServices;

namespace monoe.exe.YumSharp.Natives;

internal static partial class INative
{
  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial IntPtr YumCTable_new();

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial void YumCTable_delete(IntPtr table);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial IntPtr YumCTable_at(IntPtr table, IntPtr key);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial int YumCTable_hasKey(IntPtr table, IntPtr key);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial IntPtr YumCTable_keys(IntPtr table);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial IntPtr YumCTable_values(IntPtr table);

  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial IntPtr YumCTable_set(IntPtr table, IntPtr key, IntPtr value);


  [LibraryImport(DllName)]
  [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
  public static partial ulong YumCTable_size(IntPtr table);
}