using System;
using System.Runtime.InteropServices;

namespace monoe.exe.YumSharp.Natives;

[StructLayout(LayoutKind.Sequential)]
public struct lstring_t
{
  public IntPtr start;
  public ulong length;
  [MarshalAs(UnmanagedType.I1)]
  public bool owns;
}

[StructLayout(LayoutKind.Sequential)]
public struct binary_t
{
  public IntPtr start;
  public ulong length;
  [MarshalAs(UnmanagedType.I1)]
  public bool owns;
}

[StructLayout(LayoutKind.Sequential)]
public struct vuid_t
{
  public ulong bytes;
}

[StructLayout(LayoutKind.Sequential)]
public struct nil_t { }

// --- The union (must use Explicit layout!) ---

[StructLayout(LayoutKind.Explicit)]
public struct variant_union
{
  [FieldOffset(0)] public long integer;     // int64_t
  [FieldOffset(0)] public double number;    // double
  [FieldOffset(0)] public sbyte boolean;    // int8_t
  [FieldOffset(0)] public nil_t nil;
  [FieldOffset(0)] public vuid_t uid;
  [FieldOffset(0)] public lstring_t lstring;
  [FieldOffset(0)] public binary_t binary;
}

public enum variant_type : int
{
  VARIANT_NIL,
  VARIANT_INTEGER,
  VARIANT_NUMBER,
  VARIANT_BOOL,
  VARIANT_STRING,
  VARIANT_BINARY,
  VARIANT_UID
}

[StructLayout(LayoutKind.Sequential)]
public struct variant_t
{
  public variant_union hold; // union
  public variant_type type;  // enum
}

[StructLayout(LayoutKind.Sequential)]
public struct syserr_source_t
{
  public lstring_t func;
  public lstring_t file;
  public long line;
}

public enum syserr_category : int
{
  OK,
  ERROR,
  UNKNOWN_ERROR,
  INVALID_POINTER,
  FILE_NOT_FOUND,
  NOT_A_TABLE,
  INVALID_TYPE,
  NULL_OR_EMPTY_ARGUMENT,
  LUA_EXECUTION_ERROR,
  ILL_FUNCTION_PATH
}

[StructLayout(LayoutKind.Sequential)]
public struct syserr_t
{
  public syserr_category category;
  public syserr_source_t source;
  public lstring_t comment;
}