using System;
using Godot;

namespace monoe.exe.Core.Bridge.Types;

public class MCollisionShape2D : Exposable
{
  public static Shape2D CreateShapeFromString(string input)
  {
    // Split by colon
    string[] parts = input.Split(':');
    string type = parts[0].ToLower();

    switch (type)
    {
      case "rectangle":
        // Expect "widthxheight"
        string[] size = parts[1].Split('x');
        float w = float.Parse(size[0]);
        float h = float.Parse(size[1]);
        return new RectangleShape2D { Size = new Vector2(w, h) };

      case "circle":
        float radius = float.Parse(parts[1]);
        return new CircleShape2D { Radius = radius };

      case "capsule":
        string[] capsuleParams = parts[1].Split('x'); // "radiusxheight"
        return new CapsuleShape2D
        {
          Radius = float.Parse(capsuleParams[0]),
          Height = float.Parse(capsuleParams[1])
        };

      default:
        throw new ArgumentException($"Unknown shape type: {type}");
    }
  }

  private readonly CollisionShape2D collisionShape = new();

  public void Shape(string shape)
  {
    collisionShape.Shape = CreateShapeFromString(shape);
  }

  public void Debug(long color)
  {
    // Do nothing for now... Idk how to implement this.
  }

  public void ReShape(string shape)
  {
    collisionShape.Shape = CreateShapeFromString(shape);
  }

  protected override void _Free()
  {
    collisionShape.QueueFree();
  }

  public override Node NRef() => collisionShape;

  public override void Remove()
   => collisionShape.GetParent().RemoveChild(collisionShape);
}