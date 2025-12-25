using Godot;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types;

public class Image : ManagedObject
{
  protected Texture2D image;
  protected string path;

  public Image() {}

  public Image(Texture2D texture)
  {
    image = texture;
  }

  public void LoadImage(string path)
  {
    image = ImageTexture.CreateFromImage(Godot.Image.LoadFromFile(path));
    this.path = path;
  }

  public void Clear()
  {
    image = null;
    path = "";
  }

  public object[] GetPath() => [path];
}