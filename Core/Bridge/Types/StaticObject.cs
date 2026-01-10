using Godot;
using monoe.exe.Core.Bridge.Types.Interfaces;

namespace monoe.exe.Core.Bridge.Types;

public class StaticObject : Exposable, IPositionable2D, IScalable2D
{
  private readonly Node2D node = new();

  public object[] Deplace(double x, double y)
  {
    node.Position += new Vector2((float)x, (float)y);
    return [node.Position.X, node.Position.Y];
  }

  public object[] GetPosition()
  {
    return [node.Position.X, node.Position.Y];
  }

  public object[] GetScale()
  {
    return [node.Scale.X, node.Scale.Y];
  }

  public object[] GetSize()
  {
    return [node.Scale.X, node.Scale.Y];
  }

  public override Node NRef()
   => node;

  public void Attach(object[] uids)
  {
    foreach (var o in uids)
      if (o is long uid) Expose(node, uid);
  }

  public override void Remove()
   => node.GetParent()?.RemoveChild(node);

  public void SetPosition(double x, double y)
  {
    node.Position = new((float)x, (float)y);
  }

  public void SetScale(double x, double y)
  {
    node.Scale = new((float)x, (float)y);
  }

  public void SetSize(double x, double y)
  {
    node.Scale = new((float)x, (float)y);
  }

  object[] IScalable2D.Scale(double x, double y)
  {
    node.Scale += new Vector2((float)x, (float)y);
    return [node.Scale.X, node.Scale.Y];
  }
}