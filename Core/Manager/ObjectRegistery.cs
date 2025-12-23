using System.Collections.Concurrent;
using System.Threading;

namespace monoe.exe.Core.Manager;

public static class ObjectRegistry
{
  private static long _next = 1;
  private static ConcurrentDictionary<long, object> _objects = new();

  public static long Register(object obj)
  {
    var id = Interlocked.Increment(ref _next);
    _objects[id] = obj;
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
    var oldObjects = Interlocked.Exchange(ref _objects, new ConcurrentDictionary<long, object>());

    foreach (var obj in oldObjects.Values)
    {
      if (obj is ManagedObject mo)
      {
        try { mo.Free(); } catch { }
      }
    }
  }

}
