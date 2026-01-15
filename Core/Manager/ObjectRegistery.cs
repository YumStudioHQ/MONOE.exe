using System.Collections.Concurrent;
using System.Threading;
using Godot;
using monoe.exe.Core.Engine;

namespace monoe.exe.Core.Manager;

public static class ObjectRegistry
{
  private static long _next = 1;
  private static ConcurrentDictionary<long, object> _objects = new();

  public static long Register(object obj)
  {
    var id = Interlocked.Increment(ref _next);
    _objects[id] = obj;
    if (obj is ManagedObject mo) mo.SetUID(id);
  
    return id;
  }

  public static bool TryGet<T>(long id, out T value) where T : class
  {
    if (_objects.TryGetValue(id, out var obj) && obj is T t)
    {
      value = t;
      return true;
    }

    value = null;
    return false;
  }

  public static bool Remove(long id)
      => _objects.TryRemove(id, out _);

  public static void Clear()
  {
    EngineConsole.Verbose("clearing managed objects ...");
    var oldObjects = Interlocked.Exchange(ref _objects, new ConcurrentDictionary<long, object>());
    EngineConsole.Verbose($"{oldObjects.Count} objects will be deleted");

    foreach (var obj in oldObjects.Values)
    {
      if (obj is ManagedObject mo)
      {
        try { mo.Free(); } catch { }
      } else if (obj is Node n) n.QueueFree();

    }
    
    EngineConsole.Verbose($"{oldObjects.Count} objects have been deleted");
  }
}
