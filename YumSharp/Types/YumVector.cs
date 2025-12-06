
using System;
using System.Collections;
using System.Collections.Generic;
using monoe.exe.YumSharp.Natives;

namespace monoe.exe.YumSharp.Types;

public class YumVector : IDisposable, IEnumerable<YumVariant>
{
  internal IntPtr Handle { get; private set; }
  private readonly bool ownsHandle;

  public YumVector()
  {
    Handle = INative.YumVector_new();
    ownsHandle = true;
  }

  internal YumVector(IntPtr handle, bool ownsHandle = false)
  {
    Handle = handle;
    this.ownsHandle = ownsHandle;
  }

  public void Dispose()
  {
    if (ownsHandle && Handle != IntPtr.Zero)
    {
      INative.YumVector_delete(Handle);
      Handle = IntPtr.Zero;
    }
  }

  public void Append(YumVariant v) => INative.YumVector_append(Handle, v.Handle);

  public void Add(YumVariant v) => Append(v);

  // Sugar: collection initializer support
  public void Add(long v) => Append(new YumVariant(v));
  public void Add(double v) => Append(new YumVariant(v));
  public void Add(bool v) => Append(new YumVariant(v));
  public void Add(string v) => Append(new YumVariant(v));

  public void Pop() => INative.YumVector_pop(Handle);
  public void Clear() => INative.YumVector_clear(Handle);
  public long Count => INative.YumVector_size(Handle);

  public YumVariant this[long index]
  {
    get
    {
      var ptr = INative.YumVector_at(Handle, index);
      return new YumVariant(ptr);
    }
  }

  public IEnumerator<YumVariant> GetEnumerator()
  {
    for (long i = 0; i < Count; i++)
      yield return this[i];
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public override string ToString()
  {
    var items = new List<string>();
    foreach (var v in this)
      items.Add(v.ToString());
    return string.Join(", ", items);
  }

  public string Format(string del)
  {
    var items = new List<string>();
    foreach (var v in this)
      items.Add(v.ToString());
    return string.Join(del, items);
  }

  public static readonly YumVector UnsafeGlobalEmptyVector = [];
}
