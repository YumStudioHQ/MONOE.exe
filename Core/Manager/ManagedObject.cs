namespace monoe.exe.Core.Manager;

public abstract class ManagedObject
{
  protected long UID = -1;

  public void SetUID(long uid) => UID = uid;

  protected virtual void _Free() { }

  public void Free()
  {
    _Free();
    if (UID != -1) ObjectRegistry.Remove(UID);
  }
}
