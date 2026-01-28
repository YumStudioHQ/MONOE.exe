using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Godot;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Engine.Resources;
using monoe.exe.Core.Engine.Shell;

namespace monoe.exe.Core;

public partial class Main : Base.MainBase
{
  private static string GetBootFile()
  {
    string localProject = Path.Join(OS.GetEnvironment("PWD"), "res", "main.lua");

    if (File.Exists(localProject))
    {
      EngineConsole.Verbose($"local project found ... '{localProject}'");
      return localProject;
    }

    EngineConsole.Verbose($"local project not found ... '{localProject}'");

    var localMainPath = EngineResources.GetResourceDir("res", "main.lua");
    if (File.Exists(localMainPath))
    {
      EngineConsole.Verbose($"using built-in project '{localMainPath}'");
      return localMainPath;
    }

    return "main.lua";
  }

  private bool nr = false;

  public Main()
  {
    EngineConsole.IsVerbose = OS.GetCmdlineArgs().Contains("-dev");
    var margs = OS.GetCmdlineArgs();

    for (int i = 0; i < margs.Length; i++)
    {
      var arg = margs[i];
      if (arg == "-nr") nr = true;
      else if (arg == "-dev") Application.IsDevMode = true;
      else if (arg == "-mverb") continue;
      else
      {
        Shell.ExecuteCommand(arg.StartsWith('-') ? arg[1..] : arg, i + 1 >= margs.Length ? [] : margs[(i+1)..]);
        break;
      }
    }

    if (Application.IsDevMode) gameSettings = new()
    {
      HasHotReload = !OS.GetCmdlineArgs().Contains("-no-hot-reload"),
      HasShell = !OS.GetCmdlineArgs().Contains("-no-shell"),
      IsVerbose = !OS.GetCmdlineArgs().Contains("-silent"),
      MainFile = GetBootFile()
    }; else
    {
      gameSettings = new()
      {
        HasHotReload = false,
        HasShell = false,
        IsVerbose = OS.GetCmdlineArgs().Contains("-mverb"),
        MainFile = "res/main.lua"
      };
      Application.IsEditor = false;
    }

    Directory.SetCurrentDirectory(Directory.GetParent(gameSettings.MainFile).Parent.FullName);
    EngineConsole.Verbose($"current directory: {Directory.GetCurrentDirectory()}");
  }

  public override void _EnterTree()
  {
    if (nr)
    {
      DisplayServer.WindowSetSize(Vector2I.One);
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
