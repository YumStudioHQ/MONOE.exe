using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types;

public class LazyReadonlyBuffer : ManagedObject
{
  public object[] Hold { get; set; } = [];

  public object AtIndex(long index)
   => Hold[index - 1];
  
  public long Size() => Hold.LongLength;

  protected override void _Free()
  {
    Hold = [];
  }
}