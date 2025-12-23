namespace monoe.exe.Core.Manager;

public abstract class ManagedObject
{
  public long UID { get; }

  protected ManagedObject()
  {
    UID = ObjectRegistry.Register(this);
  }

  protected virtual void _Free() { }

  public void Free()
  {
    _Free();
    ObjectRegistry.Remove(UID);
  }
}
