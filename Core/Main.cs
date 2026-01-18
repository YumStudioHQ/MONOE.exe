using System;
using System.IO;
using System.Linq;
using Godot;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Engine.Resources;
using monoe.exe.Core.Engine.Shell;
using monoe.exe.YumSharp.Managed;

namespace monoe.exe.Core;

public partial class Main : Base.MainBase
{
  private static string GetBootFile()
  {
    if (File.Exists("res/main.lua")) return "res/main.lua";

    var localMainPath = EngineResources.GetResourceDir("res", "main.lua");
    if (File.Exists(localMainPath)) return localMainPath;

    return "main.lua";
  }

  private bool nr = false;

  public Main()
  {
    var margs = OS.GetCmdlineArgs();

    for (int i = 0; i < margs.Length; i++)
    {
      var arg = margs[i];
      if (arg == "-nr") nr = true;
      else if (arg == "-dev") continue;
      else
      {
        Shell.ExecuteCommand(arg.StartsWith('-') ? arg[1..] : arg, i + 1 >= margs.Length ? [] : margs[(i+1)..]);
        break;
      }
    }

    var query = OS.GetCmdlineArgs().Where(arg => !arg.StartsWith('-'))
                                   .Where(arg => File.Exists(arg))
                                   .ToArray();

    if (OS.GetCmdlineArgs().Contains("-dev")) gameSettings = new()
    {
      HasHotReload = !OS.GetCmdlineArgs().Contains("-no-hot-reload"),
      HasShell = !OS.GetCmdlineArgs().Contains("-no-shell"),
      IsVerbose = !OS.GetCmdlineArgs().Contains("-silent"),
      MainFile = query.Length > 0 ? query[0] : GetBootFile()
    }; else
    {
      gameSettings = new()
      {
        HasHotReload = false,
        HasShell = false,
        IsVerbose = OS.GetCmdlineArgs().Contains("-mverb"),
        MainFile = "res/main.lua"
      };
    }
  }

  public override void _EnterTree()
  {
    DisplayServer.WindowSetSize(Vector2I.One);
    if (nr)
    {
      GetTree().Quit();
    } else
    {
      base._EnterTree();
    }
  }

  public override void _Ready()
  {
    if (nr) return;
    else base._Ready();
  }
}
