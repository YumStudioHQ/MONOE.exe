using Godot;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types;

public class Image : ManagedObject
{
  public Texture2D Texture { get; protected set; }
  protected string path;

  public Image() {}

  public Image(Texture2D texture)
  {
    Texture = texture;
  }

  public void LoadImage(string path)
  {
    Texture = ImageTexture.CreateFromImage(Godot.Image.LoadFromFile(path));
    this.path = path;
  }

  public void Clear()
  {
    Texture = null;
    path = "";
  }

  public object[] GetPath() => [path];
}