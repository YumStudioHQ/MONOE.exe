using System;
using System.IO;
using System.Linq;
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

  private static bool IsAppCmd(string cmd)
  {
    return cmd == "-diagnostics" || cmd == "-mverb";
  }

  private bool nr = false;

  public Main()
  {
    EngineConsole.IsVerbose = OS.GetCmdlineArgs().Contains("-dev");
    var margs = OS.GetCmdlineArgs();
    var mainFile = GetBootFile();

    for (int i = 0; i < margs.Length; i++)
    {
      var arg = margs[i];
      if (arg == "-nr") nr = true;
      else if (arg == "-dev")
      {
        Application.IsDevMode = true;
        if (i + 1 < margs.Length)
        {
          mainFile = margs[i+1];
          i++;
        }
      }
      else if (IsAppCmd(arg)) continue;
      else if (arg == "-local-libs") Application.Libraries.Add("@PWD"); // @PWD is then expanded to the actual PWD!
      else
      {
        Shell.ExecuteCommand(arg.StartsWith('-') ? arg[1..] : arg, i + 1 >= margs.Length ? [] : margs[(i+1)..]);
        break;
      }
    }

    var pwd = Directory.GetCurrentDirectory();

    if (Directory.Exists(mainFile) && Application.IsDevMode)
    {
      Application.Libraries.Add(mainFile);
      pwd = Path.GetFullPath(mainFile);
      mainFile = Path.Combine(mainFile, "res", "main.lua");
    } 
    else if (File.Exists(mainFile) && Application.IsDevMode)
    {
      pwd = Path.GetDirectoryName(mainFile);
    }

    if (Application.IsDevMode) gameSettings = new()
    {
      HasHotReload = !margs.Contains("-no-hot-reload"),
      HasShell = !margs.Contains("-no-shell"),
      IsVerbose = !margs.Contains("-silent"),
      MainFile = mainFile,
      HasDiagnostics = margs.Contains("-diagnostics")
    }; else
    {
      gameSettings = new()
      {
        HasHotReload = false,
        HasShell = false,
        IsVerbose = margs.Contains("-mverb"),
        MainFile = "res/main.lua",
        HasDiagnostics = margs.Contains("-diagnostics")
      };

      Application.IsEditor = false;
    }

    Directory.SetCurrentDirectory(pwd);
    Application.PWD = pwd;
    EngineConsole.Verbose($"PWD: {Directory.GetCurrentDirectory()}");
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

  public override void _Notification(int what)
  {
    base._Notification(what);
  }
}
