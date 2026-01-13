using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace monoe.exe.Core.Engine.Shell;

public static class Shell
{
  private static readonly Dictionary<string, MethodInfo> commands = [];
  private static int commandRunning = 0;

  public static void Init()
  {
    EngineConsole.Verbose("starting shell...");

    var methods = typeof(BuiltIns)
      .GetMethods(BindingFlags.Public | BindingFlags.Static)
      .Select(m => (Method: m, Attr: m.GetCustomAttribute<BuiltInAttribute>()))
      .Where(x => x.Attr != null);

    foreach (var (Method, Attr) in methods) commands[Method.Name.ToLower()] = Method;
  }

  public static object[] Prompt(object[] args)
  {
    foreach (var arg in args) if (arg is string s) ExecuteCommand(s);
    return [];
  }

  public static void Prompt()
  {
    EngineConsole.Verbose("monoe shell — type `:<cmd>` in order to execute built-in command <cmd>, or, write lua code.");

    while (!AppLifetime.IsShuttingDown)
    {
      // Wait until no command is running
      while (Volatile.Read(ref commandRunning) != 0)
        Thread.Sleep(1);

      var line = EngineConsole.ReadLine("monoe> ", ConsoleColor.Cyan);

      if (line.Trim() == ":exit")
      {
        Base.MainBase.RequestExit();
        return;
      }

      if (line.StartsWith(':'))
      {
        Interlocked.Increment(ref commandRunning);

        Base.MainBase.EnqueueOnMain(() =>
        {
          try
          {
            ExecuteCommand(line);
          }
          finally
          {
            Interlocked.Decrement(ref commandRunning);
          }
        });
      }
      else
      {
        Interlocked.Increment(ref commandRunning);

        Base.MainBase.EnqueueOnMain(() =>
        {
          try
          {
            Base.MainBase.Run(line);
          }
          finally
          {
            Interlocked.Decrement(ref commandRunning);
          }
        });
      }
    }

  }

  public static void ExecuteCommand(string line)
  {
    if (line.StartsWith(':'))
    {
      var args = Parser.SplitShellArgs(line[1..]);
      if (args.Count > 0)
      {
        if (commands.TryGetValue(args[0], out MethodInfo method))
        {
          try
          {
            ((Action)method.Invoke(null, [args[1..].ToArray()]))();
          }
          catch (Exception e)
          {
            EngineConsole.WriteError(e);
          }
        }
        else
          EngineConsole.WriteError($"unknown command '{args[0]}'");
      }
    }
    else
    {
      Base.MainBase.EnqueueOnMain(() => { Base.MainBase.Run(line); });
    }
  }

  internal static void ExecuteCommand(object value)
  {
    throw new NotImplementedException();
  }
}