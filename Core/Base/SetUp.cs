using System.IO;
using Godot;
using monoe.exe.Core.Bridge.io;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Settings;

namespace monoe.exe.Core.Base;



public static class SetUp
{
  public static void Launch(Node node)
  {
    LaunchWindow(node);
    SetIcon();
    Godot.Engine.MaxFps = CurrentProject.ENGINE.MAX_FPS;
    Godot.Engine.TimeScale = CurrentProject.ENGINE.TIME_SCALE;
  }

  private static void SetIcon()
  {
    var path = PathLib.FullPath(CurrentProject.PROJECT.ICON);
    
    if (File.Exists(path)) DisplayServer.SetIcon(Image.LoadFromFile(path));
    else EngineConsole.WriteWarning($"[icon] file {path} not found!");
  }

  private static void LaunchWindow(Node node)
  {
    var win = node.GetWindow();
    win.Title = CurrentProject.WINDOW.TITLE;
    win.Size = CurrentProject.WINDOW.SIZE;
    win.Unresizable = !CurrentProject.WINDOW.RESIZABLE;
    win.Transparent = CurrentProject.WINDOW.TRANSPARENT;
    win.MaxSize = CurrentProject.WINDOW.MAX_SIZE;
    win.MinSize = CurrentProject.WINDOW.MIN_SIZE;
    win.Exclusive = CurrentProject.WINDOW.EXCLUSIVE;
    win.ExtendToTitle = CurrentProject.WINDOW.EXTEND_TO_TITLE;
  }
}