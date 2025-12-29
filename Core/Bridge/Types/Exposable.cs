using System;
using Godot;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types;

public abstract class Exposable : ManagedObject
{
  public abstract Node NRef();

  public static void Expose(Node target, long uid)
  {
    if (ObjectRegistry.TryGet<Exposable>(uid, out var mo))
    {
      target.AddChild(mo.NRef());
    }
    else
    {
      throw new ArgumentException($"{uid}: Not exposable");
    }
  }
}
