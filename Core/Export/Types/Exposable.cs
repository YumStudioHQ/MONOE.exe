using System;
using Godot;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Export.Types;

public class Exposable : ManagedObject
{
  public virtual Node NRef() => new();

  public static void Expose(Node target, long uid)
  {
    if (ObjectRegistry.TryGet<Exposable>(uid, out var mo))
    {
      target.AddChild(mo.NRef());
    }
    else throw new ArgumentException($"{uid}: Not exposable");
  }
}