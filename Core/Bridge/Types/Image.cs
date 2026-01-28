using System.Collections.Generic;
using Godot;
using monoe.exe.Core.Bridge.io;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types;

public class Image : Exposable
{
  public Texture2D Texture { get; protected set; }
  public TextureRect RenderingRect = null;
  protected string path;

  public Image() { }

  public Image(Texture2D texture)
  {
    Texture = texture;
  }

  public void LoadImage(string path)
  {
    Texture = ImageTexture.CreateFromImage(Godot.Image.LoadFromFile(PathLib.FullPath(path)));
    this.path = path;
  }

  public void Clear()
  {
    Texture = null;
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
    if (Texture == null)
      return;

    var image = Texture.GetImage();

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

    Texture = ImageTexture.CreateFromImage(image);
  }

  private void _OverlayImage(Godot.Image overlay, Vector2I position)
  {
    if (Texture == null || overlay == null)
    {
      EngineConsole.WriteWarning($"{this.GetType().FullName}.{nameof(OverlayImage)}: one of the two images is null. (skipping)");
      return;
    }

    var baseImage = Texture.GetImage();

    baseImage.Convert(Godot.Image.Format.Rgba8);
    overlay.Convert(Godot.Image.Format.Rgba8);

    var rect = new Rect2I(
      Vector2I.Zero,
      overlay.GetSize()
    );

    baseImage.BlendRect(overlay, rect, position);

    Texture = ImageTexture.CreateFromImage(baseImage);
  }

  public void OverlayImage(long uid, long x, long y)
  {
    if (ObjectRegistry.TryGet(uid, out Image image))
    {
      _OverlayImage(image.Texture.GetImage(), new((int)x, (int)y));
    }
    else throw new KeyNotFoundException($"UID: {uid} is not an image");
  }

  public string GetPath() => path;

  protected override void _Free()
  {
    RenderingRect?.QueueFree();
  }

  public override Node NRef()
  {
    RenderingRect ??= new()
    {
      Texture = Texture
    };
    return RenderingRect;
  }

  public override void Remove()
  {
    RenderingRect?.GetParent().RemoveChild(RenderingRect);
  }
}