using System;
using monoe.exe.YumSharp.Types;

namespace monoe.exe.YumSharp
{
  public static class YumVectorExtensions
  {
    // Get a slice (like Python's vec[start:end])
    public static YumVector Slice(this YumVector source, long start = 0, long? end = null)
    {
      using var result = new YumVector();

      if (source == null) 
        return result;

      long count = source.Count;
      long actualEnd = end.HasValue ? Math.Min(end.Value, count) : count;

      if (start < 0) start = count + start;
      if (start < 0) start = 0;
      if (actualEnd < 0) actualEnd = 0;
      if (actualEnd > count) actualEnd = count;
      if (start >= actualEnd) 
        return result; // empty slice

      for (long i = start; i < actualEnd; i++)
      {
        var item = source[i];
        // since YumVariant holds a handle, clone by constructing new variant with its value
        if (item.IsInt) result.Add(item.AsInt());
        else if (item.IsFloat) result.Add(item.AsFloat());
        else if (item.IsBool) result.Add(item.AsBool());
        else if (item.IsString) result.Add(item.AsString());
      }

      return result;
    }

    // Sugar for vec[start:] — no 'end'
    public static YumVector SliceFrom(this YumVector source, long start)
        => source.Slice(start, null);

    // Sugar for vec[:end]
    public static YumVector SliceTo(this YumVector source, long end)
        => source.Slice(0, end);

        public enum YumKind
    {
      Integer,
      Float,
      String,
      Bool,
      Binary,
      Table,
      UID,
      Unknown
    }

    public static YumKind GetKind(this YumVariant v)
    {
      if (v.IsInt) return YumKind.Integer;
      if (v.IsFloat) return YumKind.Float;
      if (v.IsString) return YumKind.String;
      if (v.IsBool) return YumKind.Bool;
      if (v.IsBinary) return YumKind.Binary;

      return YumKind.Unknown;
    }
  }
}