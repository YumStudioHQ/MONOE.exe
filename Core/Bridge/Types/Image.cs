using Godot;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types;

public class Image : ManagedObject
{
  public Texture2D myImage { get; protected set; }
  protected string path;

  public Image() { }

  public Image(Texture2D texture)
  {
    myImage = texture;
  }

  public void LoadImage(string path)
  {
    myImage = ImageTexture.CreateFromImage(Godot.Image.LoadFromFile(path));
    this.path = path;
  }

  public void Clear()
  {
    myImage = null;
    path = "";
  }

  private static bool Same(Color a, Color b)
  {
    return
        Mathf.Abs(a.R - b.R) < 0.01f &&
        Mathf.Abs(a.G - b.G) < 0.01f &&
        Mathf.Abs(a.B - b.B) < 0.01f &&
        Mathf.Abs(a.A - b.A) < 0.01f;
  }


  public void ReplaceColor(string fromU, string toU)
  {
    if (myImage == null)
      return;

    var image = myImage.GetImage();

    var from = Color.FromString(fromU, new Color());
    var to = Color.FromString(toU, new Color());

    for (int y = 0; y < image.GetHeight(); y++)
    {
      for (int x = 0; x < image.GetWidth(); x++)
      {
        if (Same(image.GetPixel(x, y), from))
        {
          image.SetPixel(x, y, to);
        }
      }
    }

    myImage = ImageTexture.CreateFromImage(image);
  }



  public string GetPath() => path;

  protected override void _Free()
  {
    myImage.Free();
  }
}