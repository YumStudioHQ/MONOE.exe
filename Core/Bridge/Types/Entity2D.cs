using Godot;

namespace monoe.exe.Core.Bridge.Types;

public class Entity2D : Exposable
{
  protected Node2D SelfNode;

  public Entity2D()
  {
    SelfNode = new();
  }

  public void Attach(object[] uids)
  {
    foreach (var o in uids)
      if (o is long uid) Expose(SelfNode, uid);
  }

  public void SetPosition(double x, double y)
  {
    SelfNode.Position = new((float)x, (float)y);
  }

  public object[] GetPosition()
  {
    return [SelfNode.Position.X, SelfNode.Position.Y];
  }

  public object[] Move(double x, double y)
  {
    SelfNode.Position = new(SelfNode.Position.X + (float)x, SelfNode.Position.Y + (float)y);
    return [SelfNode.Position.X, SelfNode.Position.Y];
  }

  public object[] Scale(double x, double y)
  {
    SelfNode.Scale = new((float)x, (float)y);
    return [SelfNode.Scale.X, SelfNode.Scale.Y];
  }

  protected override void _Free()
  {
    SelfNode.QueueFree();
  }

  public override Node NRef() => SelfNode;
}