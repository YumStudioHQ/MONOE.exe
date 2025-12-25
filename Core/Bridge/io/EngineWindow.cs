using System;
using Godot;
using monoe.exe.Core.Bridge;
using monoe.exe.Core.Bridge.Types;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.io;

public static class EngineWindow
{
  public static void SetTitle(string name)
   => SceneRoot.I.GetWindow().Title = name;
  
  public static object[] SetSize(double x, double y)
  {
    var vec = new Vector2I((int)x, (int)y);
    SceneRoot.I.GetWindow().Size = vec;
    return [vec.X, vec.Y];
  }
  
  public static object[] Scale(double x, double y)
  {
    var size = SceneRoot.I.GetWindow().Size;
    var nsize = new Vector2I(size.X + (int)x, size.Y + (int)y);
    SceneRoot.I.GetWindow().Size = nsize;
    return [nsize.X, nsize.Y];
  }

  public static object[] SetPosition(double x, double y)
  {
    var vec = new Vector2I((int)x, (int)y);
    SceneRoot.I.GetWindow().Position = vec;
    return [vec.X, vec.Y];
  }

  public static object[] Move(double x, double y)
  {
    var pos = SceneRoot.I.GetWindow().Position;
    var npos = new Vector2I(pos.X + (int)x, pos.Y + (int)y);
    SceneRoot.I.GetWindow().Position = npos;
    return [npos.X, npos.Y];
  }

  public static void Attach(object[] uids)
  {
    foreach (var o in uids) if (o is long uid) Exposable.Expose(SceneRoot.I, uid);
  }
}