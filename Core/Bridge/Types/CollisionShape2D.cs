using Godot;

namespace monoe.exe.Core.Bridge.Types;

public class MCollisionShape2D : Exposable
{
  private  CollisionShape2D collisionShape = new();

  public void Shape()
  {
    collisionShape = new();
  }

  public override Node NRef() => collisionShape;
}