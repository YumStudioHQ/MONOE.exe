using System.Collections.Generic;
using Godot;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Engine.Resources;

public static class ResourceManager
{
  private static readonly Dictionary<object, object> resource_dict = [];

  public static T Get<T>(object ID, T @default)
   => (T)resource_dict.GetValueOrDefault(ID, @default);  

  public static T Get<T>(object ID)
   => (T)resource_dict.GetValueOrDefault(ID, null);

  public static void Set(object key, object value) => resource_dict[key] = value;

  public static void Clear()
  {
    foreach (var (_, value) in resource_dict)
    {
      if (value is ManagedObject managed) managed.Free();
      else if (value is Node node) node.QueueFree();
    }
  }
}