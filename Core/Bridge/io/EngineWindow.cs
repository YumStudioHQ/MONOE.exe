using Godot;
using monoe.exe.Core.Bridge.Types;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.io;

public class EngineWindow
{
  private readonly Window win;

  public EngineWindow()
   => win = new();
  
  public EngineWindow(Window w) => win = w;

  public void SetTitle(string name)
   => win.Title = name;
  
  public object[] SetSize(double x, double y)
  {
    if (x > 0 && y > 0)
    {
      var vec = new Vector2I((int)x, (int)y);
      win.Size = vec;

      return [vec.X, vec.Y];
    }

    return [win.Size.X, win.Size.Y];
  }
  
  public object[] Scale(double x, double y)
  {
    var size = win.Size;
    var nsize = new Vector2I(size.X + (int)x, size.Y + (int)y);
    win.Size = nsize;
    return [nsize.X, nsize.Y];
  }

  public object[] SetPosition(double x, double y)
  {
    if (x > 0 && y > 0)
    {
      var vec = new Vector2I((int)x, (int)y);
      win.Position = vec;
      return [vec.X, vec.Y];
    }
    
    return [win.Position.X, win.Position.Y];
  }

  public object[] Move(double x, double y)
  {
    var pos = win.Position;
    var npos = new Vector2I(pos.X + (int)x, pos.Y + (int)y);
    win.Position = npos;
    return [npos.X, npos.Y];
  }

  public void Attach(object[] uids)
  {
    foreach (var o in uids) if (o is long uid) Exposable.Expose(win, uid);
  }
}

public static class EngineMainWindow
{
  public static long GetMainWindow()
   => ObjectRegistry.Register(new EngineWindow(SceneRoot.I.GetWindow()));

  public static void Attach(object[] uids)
  {
    var win = SceneRoot.I.GetWindow();
    foreach (var o in uids) if (o is long uid) Exposable.Expose(win, uid);
  }
}