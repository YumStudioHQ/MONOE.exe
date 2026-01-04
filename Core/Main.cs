using System;
using System.IO;
using System.Linq;
using Godot;
using monoe.exe.Core.Engine.Resources;

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
    var query = OS.GetCmdlineArgs().Where(arg => !arg.StartsWith('-'))
                                   .Where(arg => File.Exists(arg))
                                   .ToArray();

    if (!OS.GetCmdlineArgs().Contains("-dev")) gameSettings = new()
    {
      HasHotReload = !OS.GetCmdlineArgs().Contains("-no-hot-reload"),
      HasShell = !OS.GetCmdlineArgs().Contains("-no-shell"),
      IsVerbose = !OS.GetCmdlineArgs().Contains("-silent"),
      MainFile = query.Length > 0 ? query[0] : GetBootFile()
    };
  }
}
