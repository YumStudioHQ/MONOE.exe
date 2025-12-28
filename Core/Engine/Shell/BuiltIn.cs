using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using monoe.exe.Core.Bridge;

namespace monoe.exe.Core.Engine.Shell;

[AttributeUsage(AttributeTargets.Method)]
public class BuiltInAttribute(string doc, params string[] args) : Attribute
{
  public string Doc { get; protected set; } = doc;
  public string[] Args { get; protected set; } = args;
}

public static class BuiltIns
{
  [BuiltIn("dumps a lua table")]
  public static Action Dump(string[] args)
  {
    string code = """
                  local function dump(t)
                    for key, value in pairs(t) do
                      print(key, value)
                    end
                  end;
                  """;
    if (args.Length == 0)
    {
      code += "dump(_G);";
    }

    foreach (var table in args) code += $"dump({table});";
    return () => { Main.Run(code); };
  }

  [BuiltIn("reloads the whole project")]
  public static Action Reload(string[] _) => Main.RequestReload;

  [BuiltIn("locks / unlocks the engine's main loop. Commands can still be executed.")]
  public static Action Lock(string[] _) => Main.RequestLock;

  [BuiltIn("inspects a lua value", "<expr>")]
  public static Action Inspect(string[] args)
  {
    if (args.Length == 0) return () => {};;

    var expr = args[0];
    var code = $"""
    local v = {expr}
    if type(v) ~= "table" then
      print(type(v), v)
      return
    end
    for k, val in pairs(v) do
      print(k, type(val), val)
    end
  """;

    return () => Main.Run(code);
  }

  [BuiltIn("blocks the shell thread for N milliseconds", "ms")]
  public static Action Sleep(string[] args)
  {
    if (args.Length == 0) return () => {};;
    if (int.TryParse(args[0], out int ms))
      Thread.Sleep(ms);
    return () => {};
  }

  [BuiltIn("prints GC and memory statistics")]
  public static Action Stats(string[] _)
  {
    EngineConsole.Print(
      "Allocated:", GC.GetTotalAllocatedBytes(),
      "Total:", GC.GetTotalMemory(false)
    );
    return () => {};
  }

  [BuiltIn("lists all built-in shell commands or details one command", "[command]")]
  public static Action Help(string[] args)
  {
    var methods = typeof(BuiltIns)
      .GetMethods(BindingFlags.Public | BindingFlags.Static)
      .Select(m => (Method: m, Attr: m.GetCustomAttribute<BuiltInAttribute>()))
      .Where(x => x.Attr != null);

    if (args.Length == 0)
    {
      foreach (var (method, attr) in methods)
        EngineConsole.Print($"{method.Name.ToLower()} - {attr.Doc}");
    }
    else
    {
      var name = args[0];
      var (Method, Attr) = methods.FirstOrDefault(m =>
        m.Method.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

      if (Method == null)
      {
        EngineConsole.WriteError($"Unknown command '{name}'");
        return () => {};;
      }

      EngineConsole.Print(
        Method.Name.ToLower(),
        Attr.Doc,
        "Args:", string.Join(" ", Attr.Args)
      );
    }
    return () => {};
  }

  [BuiltIn("clears the console")]
  public static Action Clear(string[] _)
  {
    Console.Clear();
    return () => {};
  }

  [BuiltIn("quits the engine")]
  public static Action Exit(string[] _)
    => Main.RequestExit;

  public static Action Emit(string[] args)
  {
    if (args.Length == 0)
    {
      EngineConsole.WriteError("emit requires an event name");
      return () => {};
    }

    var eventName = args[0];
    var parsed = new List<object>();

    for (int i = 1; i < args.Length; i++)
    {
      parsed.Add(ParseArg(args[i]));
    }

    return () =>
    {
      Main.Emit(eventName, [.. parsed]);
    };
  }

  private static object ParseArg(string arg)
  {
    if (string.IsNullOrEmpty(arg))
      return "";

    if (
      (arg.StartsWith('"') && arg.EndsWith('"')) ||
      (arg.StartsWith('\'') && arg.EndsWith('\''))
    )
    {
      return arg[1..^1];
    }

    if (bool.TryParse(arg, out var b))
      return b;

    if (long.TryParse(arg, out var l))
      return l;

    if (double.TryParse(
          arg,
          System.Globalization.NumberStyles.Float,
          System.Globalization.CultureInfo.InvariantCulture,
          out var d))
      return d;

    if (arg.Trim() == "nil" || arg.Trim() == "null") return null;

    return arg;
  }
}