using System.Collections.Generic;
using Godot;
using monoe.exe.YumSharp.Types;

namespace monoe.exe.YumSharp;

public static class VariantConverter
{
  public static YumVariant ToYumVariant(Variant v)
  {
    switch (v.VariantType)
    {
      case Variant.Type.Bool:
        return new YumVariant(v.As<bool>());

      case Variant.Type.Int:
        return new YumVariant((long)v);

      case Variant.Type.Float:
        return new YumVariant((double)v);

      case Variant.Type.String:
        return new YumVariant(v.As<string>());

      case Variant.Type.Vector3I:
        {
          var vec = v.As<Vector3I>();
          var yumVec = new YumVector
          {
            (long)vec.X,
            (long)vec.Y,
            (long)vec.Z
          };
          return new YumVariant(yumVec.ToString());
        }

      case Variant.Type.Vector3:
        {
          var vec = v.As<Vector3>();
          var yumVec = new YumVector
          {
            (long)vec.X,
            (long)vec.Y,
            (long)vec.Z
          };
          return new YumVariant(yumVec.ToString());
        }

      case Variant.Type.Array:
        {
          var arr = v.As<Godot.Collections.Array>();
          var yumVec = new YumVector();
          foreach (var item in arr)
            yumVec.Add(ToYumVariant(item));
          return new YumVariant(yumVec.ToString());
        }

      default:
        return new YumVariant(v.ToString());
    }
  }

  public static YumVector ToYumVector(Godot.Collections.Array args)
  {
    var yumVec = new YumVector();
    foreach (var v in args)
      yumVec.Add(ToYumVariant(v));
    return yumVec;
  }

  public static YumVector ToYumVector(Variant[] args)
  {
    var yumVec = new YumVector();
    foreach (var v in args)
      yumVec.Add(ToYumVariant(v));
    return yumVec;
  }

  public static Variant ToVariant(YumVariant variant)
  {
    Variant me = new();
    if (variant.IsInt) me = variant.AsInt();
    else if (variant.IsFloat) me = variant.AsFloat();
    else if (variant.IsBool) me = variant.AsBool();
    else if (variant.IsString) me = variant.AsString();

    return me;
  }

  public static Variant ToVariant(YumVector vector)
  {
    Variant v = new();
    Godot.Collections.Array list = [];

    foreach (var variant in vector) list.Add(ToVariant(variant));

    return v;
  }

  public static Variant[] ToVariantArray(YumVector vector)
  {
    List<Variant> list = [];

    foreach (var variant in vector) list.Add(ToVariant(variant));

    return [.. list];
  }

  public static Variant[] ToVariantArray(YumVariant[] vector)
  {
    List<Variant> list = [];

    foreach (var variant in vector) list.Add(ToVariant(variant));

    return [.. list];
  }
}
