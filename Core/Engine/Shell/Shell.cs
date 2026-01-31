using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace monoe.exe.Core.Engine.Shell;

public static class Shell
{
  private static readonly Dictionary<string, Action<string[]>> commands = [];
  private static int commandRunning = 0;

  static Shell()
  {
    var types = EngineAssembly.GetTypes()
        .Where(t => t.GetCustomAttribute<ShellCommandHolderAttribute>() != null);

    foreach (var type in types)
    {
      var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                        .Where(m => m.GetCustomAttribute<ShellCommandAttribute>() != null);

      foreach (var method in methods)
      {
        var attr = method.GetCustomAttribute<ShellCommandAttribute>();
        if (attr != null)
        {
          void action(string[] args)
          {
            try
            {
              object instance = method.IsStatic ? null : Activator.CreateInstance(type);
              method.Invoke(instance, [args]);
            }
            catch (Exception e)
            {
              EngineConsole.WriteError(e);
            }
          }

          commands[attr.Name] = action;
        }
      }
    }
  }

  public static object[] Prompt(object[] args)
  {
    foreach (var arg in args)
      if (arg is string s) ExecuteCommand(s);

    return [];
  }

  public static void Prompt()
  {
    EngineConsole.Verbose("monoe shell — type `:<cmd>` to execute commands, or write Lua code.");

    while (!Application.IsShuttingDown)
    {
      while (Volatile.Read(ref commandRunning) != 0)
        Thread.Sleep(1);

      var line = EngineConsole.ReadLine("monoe> ", ConsoleColor.Cyan);

      if (line.Trim() == ":exit")
      {
        Base.MainBase.RequestExit();
        return;
      }

      Interlocked.Increment(ref commandRunning);

      Base.MainBase.EnqueueOnMain(() =>
      {
        try
        {
          if (line.StartsWith(':'))
            ExecuteCommand(line);
          else
            Base.MainBase.Run(line);
        }
        finally
        {
          Interlocked.Decrement(ref commandRunning);
        }
      });
    }
  }

  public static void ExecuteCommand(string line)
  {
    if (!line.StartsWith(':')) return;

    var args = Parser.SplitShellArgs(line[1..]);
    if (args.Count == 0) return;

    if (commands.TryGetValue(args[0], out var action))
      action([.. args.Skip(1)]);
    else
      EngineConsole.WriteError($"unknown command '{args[0]}'");
  }

  public static void ExecuteCommand(string cmd, params string[] args)
  {
    if (commands.TryGetValue(cmd, out var action))
      action(args);
    else
      EngineConsole.WriteError($"unknown command '{cmd}'");
  }
}
