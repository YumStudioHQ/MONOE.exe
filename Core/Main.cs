using System;
using System.IO;
using System.Linq;
using Godot;
using monoe.exe.Core.Engine.Runtime;

namespace monoe.exe.Core;

public partial class Main : Base.MainBase
{
  public Main()
  {
    var query = OS.GetCmdlineArgs().Where(arg => !arg.StartsWith('-'))
                                   .Where(arg => File.Exists(arg))
                                   .ToArray();

    if (!EngineRuntimeInformations.IsRelease) gameSettings = new()
    {
      HasHotReload = !OS.GetCmdlineArgs().Contains("-no-hot-reload"),
      HasShell = !OS.GetCmdlineArgs().Contains("-no-shell"),
      IsVerbose = !OS.GetCmdlineArgs().Contains("-silent"),
      MainFile = query.Length > 0 ? query[0] : (File.Exists("res/main.lua") ? "res/main.lua" : "main.lua")
    };
    else gameSettings = new();
  }
}
