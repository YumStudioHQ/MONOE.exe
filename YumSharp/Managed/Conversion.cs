using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Godot;
using monoe.exe.YumSharp.Natives;

namespace monoe.exe.YumSharp.Managed;

public static partial class Conversion
{
  public static string LStringToString(lstring_t ls)
  {
    if (ls.start == IntPtr.Zero || ls.length == 0)
      return string.Empty;

    byte[] buffer = new byte[(int)ls.length];
    Marshal.Copy(ls.start, buffer, 0, (int)ls.length);

    return Encoding.UTF8.GetString(buffer);
  }

  public static byte[] GetBytesFromCxx(binary_t bin)
  {
    if (bin.start == nint.Zero || bin.length == 0)
      return [];

    byte[] buffer = new byte[(int)bin.length];
    Marshal.Copy(bin.start, buffer, 0, (int)bin.length);

    return buffer;
  }

  public static unsafe variant_t ObjectToVariant(
    object o,
    List<byte[]> pinnedStrings // keeps UTF8 buffers alive
  )
  {
    variant_t v = default;

    switch (o)
    {
      case null:
        v.type = variant_type.VARIANT_NIL;
        break;

      case short i:
        v.type = variant_type.VARIANT_INTEGER;
        v.hold.integer = i;
        break;

      case ushort i:
        v.type = variant_type.VARIANT_INTEGER;
        v.hold.integer = i;
        break;

      case int i:
        v.type = variant_type.VARIANT_INTEGER;
        v.hold.integer = i;
        break;

      case uint i:
        v.type = variant_type.VARIANT_INTEGER;
        v.hold.integer = i;
        break;

      case long l:
        v.type = variant_type.VARIANT_INTEGER;
        v.hold.integer = l;
        break;

      case float f:
        v.type = variant_type.VARIANT_NUMBER;
        v.hold.number = f;
        break;

      case double d:
        v.type = variant_type.VARIANT_NUMBER;
        v.hold.number = d;
        break;

      case bool b:
        v.type = variant_type.VARIANT_BOOL;
        v.hold.boolean = (sbyte)(b ? 1 : 0);
        break;

      case string s:
        {
          byte[] utf8 = Encoding.UTF8.GetBytes(s);
          pinnedStrings.Add(utf8); // keep alive

          fixed (byte* p = utf8)
          {
            v.type = variant_type.VARIANT_STRING;
            v.hold.lstring = new lstring_t
            {
              start = (IntPtr)p,
              length = (ulong)utf8.Length,
              owns = false
            };
          }
          break;
        }

      case char[] s:
        {
          byte[] utf8 = Encoding.UTF8.GetBytes(s);
          pinnedStrings.Add(utf8); // keep alive

          fixed (byte* p = utf8)
          {
            v.type = variant_type.VARIANT_STRING;
            v.hold.lstring = new lstring_t
            {
              start = (IntPtr)p,
              length = (ulong)utf8.Length,
              owns = false
            };
          }
          break;
        }

      case List<char> s:
        {
          byte[] utf8 = Encoding.UTF8.GetBytes([..s]);
          pinnedStrings.Add(utf8); // keep alive

          fixed (byte* p = utf8)
          {
            v.type = variant_type.VARIANT_STRING;
            v.hold.lstring = new lstring_t
            {
              start = (IntPtr)p,
              length = (ulong)utf8.Length,
              owns = false
            };
          }
          break;
        }

      default:
        throw new NotSupportedException($"Unsupported type: {o.GetType()}");
    }

    return v;
  }

  public static object VariantToObject(variant_t var)
  {
    return var.type switch
    {
      variant_type.VARIANT_STRING => LStringToString(var.hold.lstring),
      variant_type.VARIANT_BINARY => GetBytesFromCxx(var.hold.binary),
      variant_type.VARIANT_INTEGER => var.hold.integer,
      variant_type.VARIANT_NUMBER => var.hold.number,
      variant_type.VARIANT_BOOL => var.hold.boolean == 1,
      variant_type.VARIANT_UID => var.hold.uid,
      _ => null,
    };
  }
}
