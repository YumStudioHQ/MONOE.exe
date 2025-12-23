using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace monoe.exe.Core.Export.Types;

public class Object
{
  private static readonly ConcurrentDictionary<long, Object> objects = new();
  private static long last = 0;

  private readonly long uid;

  public Object()
  {
    uid = Interlocked.Increment(ref last);
    objects.TryAdd(uid, this);
  }

  public long Ref() => uid;

  public void Free()
  {
    if (!Remove(uid))
      throw new Exception($"Cannot free object #{uid}");
  }

  protected static bool TryGet(long id, out Object obj)
  {
    return objects.TryGetValue(id, out obj);
  }

  protected static Object Get(long id)
  {
    if (objects.TryGetValue(id, out var obj))
      return obj;

    throw new KeyNotFoundException($"Object {id} not found");
  }

  protected static void AddOrReplace(Object obj)
  {
    objects.AddOrUpdate(
        obj.uid,
        obj,
        (_, _) => obj
    );
  }

  protected static bool Remove(long id)
  {
    return objects.TryRemove(id, out _);
  }

  protected static Object[] All()
  {
    return [.. objects.Values];
  }
}
