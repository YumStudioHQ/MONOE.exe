using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using monoe.exe.Core.Base;
using monoe.exe.Core.Engine.Resources;
using monoe.exe.Core.Engine.Shell;

namespace monoe.exe.Core.Engine;

[ShellCommandHolder]
public static class Application
{
  internal static volatile bool IsShuttingDown = false;
  internal static volatile bool IsDevMode = OS.GetCmdlineArgs().Contains("-dev");
  internal static volatile bool IsEditor = true;

  [ShellCommand("exit", help: "Quits the engine")]
  public static void Exit(string[]_)
  {
    MainBase.RequestExit();
  }

  public static string ProjectSettingsFile()
  {
    return Path.Combine(
      EngineResources.GetRuntimeResourceDir(),
      "project.lua"
    );
  }
}
