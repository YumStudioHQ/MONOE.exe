using Godot;
using monoe.exe.Core.Bridge;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Export.Types;

public class Sprite : ManagedObject
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

  public void Render()
  {
    SceneRoot.I.AddChild(sprite);
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

  protected override void _Free()
  {
    sprite.QueueFree();
  }
}