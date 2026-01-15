using System;

namespace monoe.exe.Core.Manager;

public abstract class ManagedObject : IDisposable
{
  protected long UID = -1;
  private bool freed = false;

  public void SetUID(long uid) => UID = uid;

  protected abstract void _Free();

  public void Free()
  {
    if (!freed)
    {
      _Free();
      if (UID != -1) ObjectRegistry.Remove(UID);
      freed = true;
    }
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
    Free();
  }
}
