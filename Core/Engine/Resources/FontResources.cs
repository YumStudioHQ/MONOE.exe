using System.IO;
using Godot;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Engine.Resources;

public class FontResource : ManagedObject
{
  private readonly Font font;

  public FontResource(string path)
  {
    var res = ResourceManager.Get<FontResource>(path);
    if (res == null)
    {
      FontFile file = new();
      var err = file.LoadDynamicFont(path);
      if (err != Error.Ok) throw new FileLoadException($"cannot load file font {path}");
      font = file;
      return;
    }

    font = res.GetFont();
    ResourceManager.Set(path, this);
  }

  public Font GetFont() => font;

  protected override void _Free() => font?.Free();
}