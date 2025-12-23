using Godot;
using monoe.exe.Core.Bridge;

namespace monoe.exe.Core.Export.Types;

public class Sprite : Exposable
{
  private readonly Sprite2D sprite = new();

  public void LoadImage(string path)
  {
    sprite.Texture = ImageTexture.CreateFromImage(Image.LoadFromFile(path));
  }

  public void Clear()
  {
    sprite.Texture = null;
  }

  public void SetPosition(double x, double y)
  {
    sprite.Position = new((float)x, (float)y);
  }

  public object[] GetPosition()
  {
    return [sprite.Position.X, sprite.Position.Y];
  }

  public object[] Move(double x, double y)
  {
    sprite.Position = new(sprite.Position.X + (float)x, sprite.Position.Y + (float)y);
    return [sprite.Position.X, sprite.Position.Y];
  }

  public object[] Scale(double x, double y)
  {
    sprite.Scale = new((float)x, (float)y);
    return [sprite.Scale.X, sprite.Scale.Y];
  }

  protected override void _Free()
  {
    sprite.QueueFree();
  }

  public override Node NRef() => sprite;
}