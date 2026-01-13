using Godot;
using monoe.exe.Core.Bridge.Types.Interfaces;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types;

public class Sprite : Exposable, IPositionable2D, IScalable2D
{
  private readonly Sprite2D sprite = new();

  public void LoadImage(string path)
  {
    sprite.Texture = ImageTexture.CreateFromImage(Godot.Image.LoadFromFile(path));
  }

  public void Clear()
  {
    sprite.Texture = null;
  }

  public void SetPosition(double x, double y)
  {
    sprite.GlobalPosition = new((float)x, (float)y);
  }

  public object[] GetPosition()
  {
    return [sprite.GlobalPosition.X, sprite.GlobalPosition.Y];
  }

  public object[] Deplace(double x, double y)
  {
    sprite.GlobalPosition = new(sprite.GlobalPosition.X + (float)x, sprite.GlobalPosition.Y + (float)y);
    return [sprite.GlobalPosition.X, sprite.GlobalPosition.Y];
  }

  public object[] Scale(double x, double y)
  {
    sprite.Scale = new(sprite.Scale.X + (float)x, sprite.Scale.Y + (float)y);
    return [sprite.Scale.X, sprite.Scale.Y];
  }

  public void SetSize(double x, double y)
  {
    sprite.Scale = new((float)x, (float)y);
  }

  public object[] GetSize() => [sprite.Scale.X, sprite.Scale.Y];

  public void SetScale(double x, double y) => Scale(x, y);
  public object[] GetScale() => [sprite.Scale.X, sprite.Scale.Y];

  protected override void _Free()
  {
    sprite.QueueFree();
  }

  public override Node NRef() => sprite;

  public long GetImageUID()
  {
    long uid = ObjectRegistry.Register(new Image(sprite.Texture));
    return uid;
  }

  public override void Remove()
   => sprite.GetParent().RemoveChild(sprite);
}