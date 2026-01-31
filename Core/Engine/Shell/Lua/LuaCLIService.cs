using System;
using System.IO;
using System.Linq;
using Godot;
using monoe.exe.YumSharp.Managed;

namespace monoe.exe.Core.Engine.Shell.Lua;

[ShellCommandHolder]
public static class LuaCLIService
{
  public static bool IsLuaFile(string input)
  {
    return File.Exists(input) &&
           Path.GetExtension(input).Equals(".lua", StringComparison.OrdinalIgnoreCase);
  }

  [ShellCommand("lua", "Execute a lua file or lua code. If no arguments are provided, an interactive shell starts.", ["[files?|code?]", "[-cli: starts the CLI after running given files/code]"])]
  public static void Lua(string[] args)
  {
    using YumState state = new(true);
    var startsShell = false;
    var margs = args.Where(str => !string.IsNullOrWhiteSpace(str)).ToArray();
    if (margs.Length < 1) startsShell = true;
    else
    {
      foreach (var arg in margs)
      {
        if (arg.Trim() == "-cli") startsShell = true;
        state.Run(arg, IsLuaFile(arg));
      }
    }

    if (startsShell) LuaCLI(state);
  }

  public static void LuaCLI(YumState state)
  {
    EngineConsole.WriteLine($"monoe lua -- based on monoe.exe@{Version.All}");
    while (!Application.IsShuttingDown)
    {
      var line = EngineConsole.ReadLine("> ", System.ConsoleColor.Green);
      if (line.Trim() == ":exit") return;
      try
      {
        state.Run(line, false);
      }
      catch (YumException e)
      {
        EngineConsole.WriteError(e.Message);
      }
    }
  }
}