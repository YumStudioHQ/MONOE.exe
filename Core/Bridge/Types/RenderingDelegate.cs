using Godot;

namespace monoe.exe.Core.Bridge.Types;

public class RenderingDelegate : Exposable
{
  private readonly Node node = new();

  public override Node NRef() => node;

  public void Attach(object[] uids)
  {
    foreach (var o in uids)
      if (o is long uid) Expose(node, uid);
  }

  public override void Remove()
   => node.GetParent().RemoveChild(node);
}