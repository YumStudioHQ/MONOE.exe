namespace monoe.exe.Core.Manager;

public abstract class ManagedObject
{
  public long UID { get; }

  protected ManagedObject()
  {
    UID = ObjectRegistry.Register(this);
  }

  public virtual void Free()
  {
    ObjectRegistry.Remove(UID);
  }
}
