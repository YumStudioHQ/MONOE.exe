using System.Collections.Generic;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types.Internals;

public class Dict : ManagedObject
{
  private Dictionary<string, object> values = [];

  public void Set(string key, object value) => values[key] = value;
  public object Get(string key, object @default = null) => values.GetValueOrDefault(key, @default);
  public void Clear() => values = [];

  protected override void _Free()
  {
    values = [];
  }
}