using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using monoe.exe.Core.Base;
using monoe.exe.Core.Engine.Resources;

namespace monoe.exe.Core.Engine;

public static class Application
{
  public static volatile bool IsShuttingDown = false;
  public static volatile bool IsDevMode = OS.GetCmdlineArgs().Contains("-dev");
  public static volatile bool IsEditor = true;
  internal static string PWD = Directory.GetCurrentDirectory();

  public static List<string> Libraries { get; set; } = [
    EngineResources.GetResourceDir(),
  ];

  public static void Exit(long code)
  {
    EngineConsole.Verbose("exit requested by internals...");
    MainBase.RequestExit((int)code);
    MainBase.Lsleep([10]);
  }
}
