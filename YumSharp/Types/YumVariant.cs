using System;
using System.Runtime.InteropServices;
using System.Text;
using monoe.exe.YumSharp.Natives;

namespace monoe.exe.YumSharp.Types;

public class YumVariant : IDisposable
{
  internal IntPtr Handle { get; private set; }

  public YumVariant()
  {
    Handle = INative.YumVariant_new();
  }

  public YumVariant(long v) : this() => Set(v);
  public YumVariant(double v) : this() => Set(v);
  public YumVariant(bool v) : this() => Set(v);
  public YumVariant(string v) : this() => Set(v);
  public YumVariant(byte[] v) : this() => Set(v);
  public YumVariant(YumUID uid) : this() => Set(uid);
  public YumVariant(YumTable table) : this() => Set(table);

  internal YumVariant(IntPtr handle)
  {
    Handle = handle;
  }

  public void Set(long v) => INative.YumVariant_setInt(Handle, v);
  public void Set(double v) => INative.YumVariant_setFloat(Handle, v);
  public void Set(bool v) => INative.YumVariant_setBool(Handle, v ? 1 : 0);
  public void Set(string v) => INative.YumVariant_setString(Handle, v);
  public void Set(byte[] v)
  {
    if (v == null || v.Length == 0)
    {
      INative.YumVariant_setRawBytes(Handle, new(IntPtr.Zero, 0));
      return;
    }

    unsafe
    {
      fixed (byte* ptr = v)
      {
        INative.YumVariant_setRawBytes(Handle, new((IntPtr)ptr, (ulong)v.Length));
      }
    }
  }

  public void Set(YumUID uid) => INative.YumVariant_setUid(Handle, uid);
  public void Set(YumTable table) => INative.YumVariant_setTable(Handle, table.Handle);

  public long AsInt()
  {
    if (INative.YumVariant_isInt(Handle) == 0) return 0;
    return INative.YumVariant_asInt(Handle);
  }

  public double AsFloat()
  {
    if (INative.YumVariant_isFloat(Handle) == 0) return 0.0;
    return INative.YumVariant_asFloat(Handle);
  }

  public bool AsBool()
  {
    if (INative.YumVariant_isBool(Handle) == 0) return false;
    return INative.YumVariant_asBool(Handle) != 0;
  }

  public string AsString()
  {
    if (INative.YumVariant_isString(Handle) == 0) return "";
    return INative.YumVariant_asStringSafe(Handle);
  }

  public byte[] AsBytes()
  {
    if (INative.YumVariant_isBinary(Handle) == 0) return [];
    var blob = INative.YumVariant_asBinary(Handle);
    var data = new byte[blob.size];
    Marshal.Copy(blob.start, data, 0, (int)blob.size);
    return data;
  }

  public YumUID AsUID()
  {
    if (INative.YumVariant_isUid(Handle) == 0) return new();
    return INative.YumVariant_asUID(Handle);
  }

  public YumTable AsTable()
  {
    if (INative.YumVariant_isTable(Handle) == 0) return new();
    return new(INative.YumVariant_asTable(Handle));
  }

  public bool IsInt => INative.YumVariant_isInt(Handle) != 0;
  public bool IsFloat => INative.YumVariant_isFloat(Handle) != 0;
  public bool IsBool => INative.YumVariant_isBool(Handle) != 0;
  public bool IsString => INative.YumVariant_isString(Handle) != 0;
  public bool IsBinary => INative.YumVariant_isBinary(Handle) != 0;
  public bool IsUID => INative.YumVariant_asInt(Handle) != 0;
  public bool IsTable => INative.YumVariant_asTable(Handle) != 0;

  // --- Implicit conversions ---
  public static implicit operator YumVariant(long v) => new(v);
  public static implicit operator YumVariant(double v) => new(v);
  public static implicit operator YumVariant(bool v) => new(v);
  public static implicit operator YumVariant(string v) => new(v);
  public static implicit operator YumVariant(byte[] v) => new(v);
  public static implicit operator YumVariant(YumUID uid) => new(uid);
  public static implicit operator YumVariant(YumTable table) => new(table);

  public static implicit operator long(YumVariant v) => v.AsInt();
  public static implicit operator double(YumVariant v) => v.AsFloat();
  public static implicit operator bool(YumVariant v) => v.AsBool();
  public static implicit operator string(YumVariant v) => v.AsString();
  public static implicit operator byte[](YumVariant v) => v.AsBytes();
  public static implicit operator YumUID(YumVariant v) => v.AsUID();
  public static implicit operator YumTable(YumVariant v) => v.AsTable();

  public override string ToString()
  {
    if (IsInt) return AsInt().ToString();
    if (IsFloat) return AsFloat().ToString();
    if (IsBool) return AsBool().ToString();
    if (IsString) return AsString();
    if (IsBinary) return new string(Encoding.UTF8.GetChars(AsBytes()));
    if (IsUID) return $"YumUID#{AsUID().bytes}";
    if (IsTable) return $"YumTable";
    return "<nil>";
  }

  public string AsLiteralValue()
  {
    if (IsString) return $"\"{AsString()}\"";
    return ToString();
  }

  public void Dispose()
  {
    if (Handle != IntPtr.Zero)
    {
      INative.YumVariant_delete(Handle);
      Handle = IntPtr.Zero;
    }
  }
}