using Godot;
using monoe.exe.Core.Bridge.Types.Interfaces;

namespace monoe.exe.Core.Bridge.Types;

public class Entity2D : Exposable, IPositionable2D, IScalable2D
{
  protected CharacterBody2D node;

  public Entity2D()
  {
    node = new();
  }

  public void Attach(object[] uids)
  {
    foreach (var o in uids)
      if (o is long uid) Expose(node, uid);
  }

  public void SetPosition(double x, double y)
  {
    node.Position = new((float)x, (float)y);
  }

  public object[] GetPosition()
  {
    return [node.Position.X, node.Position.Y];
  }

  public object[] Deplace(double x, double y)
  {
    node.Position = new(node.Position.X + (float)x, node.Position.Y + (float)y);
    return [node.Position.X, node.Position.Y];
  }

  public object[] Scale(double x, double y)
  {
    node.Scale = new((float)x, (float)y);
    return [node.Scale.X, node.Scale.Y];
  }

  public void Velocity(double x, double y)
  {
    node.Velocity = new((float)x * 100, (float)y * 100);
    // TODO? 
    //  Might add a way to change these values at runtime if possible and necessary.
  }

  public void MoveAndSlide()
  {
    node.MoveAndSlide();
  }

  public void SetSize(double x, double y)
  {
    node.Scale = new((float)x, (float)y);
  }

  public object[] GetSize() => [node.Scale.X, node.Scale.Y];

  public void SetScale(double x, double y) => Scale(x, y);
  public object[] GetScale() => [node.Scale.X, node.Scale.Y];

  protected override void _Free()
  {
    node.QueueFree();
  }

  public override Node NRef() => node;
  
  public override void Remove()
   => node.GetParent().RemoveChild(node);
}