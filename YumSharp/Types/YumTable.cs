using System;
using System.Collections;
using System.Collections.Generic;
using monoe.exe.YumSharp.Natives;

namespace monoe.exe.YumSharp.Types;

public class YumTable : IDisposable, IEnumerable<KeyValuePair<YumVariant, YumVariant>>
{
  public IntPtr Handle { get; private set; }

  public YumVariant this[YumVariant key]
  {
    get
    {
      if (INative.YumCTable_hasKey(Handle, key.Handle) != 0) return INative.YumCTable_at(Handle, key.Handle);
      throw new KeyNotFoundException($"Invalid key: {key}");
    }
    set
    {
      INative.YumCTable_set(Handle, key.Handle, value.Handle);
    }
  }

  public YumTable()
  {
    Handle = INative.YumCTable_new();
  }

  public YumTable(IntPtr table)
  {
    Handle = table; // THIS MIGHT BE REVIEWED 
  }

  public void Dispose()
  {
    INative.YumCTable_delete(Handle);
  }

  public IEnumerator<KeyValuePair<YumVariant, YumVariant>> GetEnumerator()
  {
    var keys = new YumVector(INative.YumCTable_keys(Handle));
    var values = new YumVector(INative.YumCTable_values(Handle));

    long count = keys.Count;
    for (long i = 0; i < count; i++)
    {
      yield return new KeyValuePair<YumVariant, YumVariant>(keys[i], values[i]);
    }

    keys.Dispose();
    values.Dispose();
  }

  public ulong Length()
  {
    return INative.YumCTable_size(Handle);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}