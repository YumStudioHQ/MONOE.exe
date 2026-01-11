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
  private string GetBootFile()
  {
    if (File.Exists("res/main.lua")) return "res/main.lua";

    var localMainPath = EngineResources.GetResourceDir("res", "main.lua");
    if (File.Exists(localMainPath)) return localMainPath;

    return "main.lua";
  }

  public Main()
  {
    if (OS.GetCmdlineArgs().Contains("-cli"))
    {
      Shell.Init();
      var argl = string.Join(' ', OS.GetCmdlineArgs().Where(arg => arg != "-cli"));
      var args = argl.Split(',');
      foreach (var arg in args)
      {
        if (arg.StartsWith('-')) Shell.ExecuteCommand(string.Concat(":", arg.AsSpan(1)));
        else Shell.ExecuteCommand(arg);
      }
      GetTree().Quit(0);
    } else if (OS.GetCmdlineArgs().Contains("-file"))
    {
      var file = OS.GetCmdlineArgs().Where(arg => !arg.StartsWith('-'))
                                    .Where(arg => File.Exists(arg))
                                    .FirstOrDefault("main.lua");
      YumState state = new(true);
      try
      {
        state.Run(file, true);
        GetTree().Quit(0);
      }
      catch (Exception e)
      {
        EngineConsole.WriteError(e);
        GetTree().Quit(1);
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
}
