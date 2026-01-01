using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using monoe.exe.Core.Bridge;
using monoe.exe.Core.Engine.Compiler;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Engine.Shell;

[AttributeUsage(AttributeTargets.Method)]
public class BuiltInAttribute(string doc, params string[] args) : Attribute
{
  public string Doc { get; protected set; } = doc;
  public string[] Args { get; protected set; } = args.Length > 0 ? args : ["<none>"];
}

public static class BuiltIns
{
  [BuiltIn("dumps a lua table", "<table> (default: _G)")]
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
    if (args.Length == 0) return () => { }; ;

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
    if (args.Length == 0) return () => { }; ;
    if (int.TryParse(args[0], out int ms))
      Thread.Sleep(ms);
    return () => { };
  }

  [BuiltIn("prints GC and memory statistics")]
  public static Action Stats(string[] _)
  {
    EngineConsole.Print(
      "Allocated:", GC.GetTotalAllocatedBytes(),
      "Total:", GC.GetTotalMemory(false)
    );
    return () => { };
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
        EngineConsole.WriteLine($"{method.Name.ToLower()} - {attr.Doc}", ConsoleColor.DarkBlue);
    }
    else
    {
      var name = args[0];
      var (Method, Attr) = methods.FirstOrDefault(m =>
        m.Method.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

      if (Method == null)
      {
        EngineConsole.WriteError($"Unknown command '{name}'");
        return () => { }; ;
      }

      EngineConsole.WriteLine(
        $"{Method.Name.ToLower()}\t" +
        $"{Attr.Doc}\t" +
        $"Args: {string.Join(" ", Attr.Args)}", ConsoleColor.DarkBlue
      );
    }
    return () => { };
  }

  [BuiltIn("clears the console")]
  public static Action Clear(string[] _)
  {
    Console.Clear();
    return () => { };
  }

  [BuiltIn("quits the engine")]
  public static Action Exit(string[] _)
    => Main.RequestExit;

  [BuiltIn("emits an event with given arguments")]
  public static Action Emit(string[] args)
  {
    if (args.Length == 0)
    {
      EngineConsole.WriteError("emit requires an event name");
      return () => { };
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

  [BuiltIn("shows detailed info about a C# object based on its UID.", "[IDs...]")]
  public static Action Object(string[] args)
  {
    return () =>
    {
      foreach (var struid in args)
      {
        if (!long.TryParse(struid, out long uid))
        {
          EngineConsole.WriteError($"[!] Expected integer UID, got '{struid}'");
          continue;
        }

        if (!ObjectRegistry.TryGet(uid, out object o))
        {
          EngineConsole.WriteError($"[!] No object found for UID {uid}");
          continue;
        }

        var type = o.GetType();
        var baseType = type.BaseType;
        var assembly = type.Assembly.GetName().Name;
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Select(p => $"{p.Name} ({p.PropertyType.Name})")
                             .ToArray();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                         .Select(f => $"{f.Name} ({f.FieldType.Name})")
                         .ToArray();
        var interfaces = type.GetInterfaces().Select(i => i.Name).ToArray();

        EngineConsole.WriteLine(new string('─', 40), ConsoleColor.DarkGray);
        EngineConsole.WriteLine($"UID: {uid}", ConsoleColor.Cyan);
        EngineConsole.WriteLine($"Type: {type.FullName}", ConsoleColor.Green);
        EngineConsole.WriteLine($"Base: {baseType?.FullName ?? "none"}", ConsoleColor.Yellow);
        EngineConsole.WriteLine($"Assembly: {assembly}", ConsoleColor.Magenta);

        if (interfaces.Length > 0)
          EngineConsole.WriteLine($"Implements: {string.Join(", ", interfaces)}", ConsoleColor.Blue);

        if (properties.Length > 0)
          EngineConsole.WriteLine($"Properties: {string.Join(", ", properties)}", ConsoleColor.White);

        if (fields.Length > 0)
          EngineConsole.WriteLine($"Fields: {string.Join(", ", fields)}", ConsoleColor.Gray);

        EngineConsole.WriteLine(new string('─', 40), ConsoleColor.DarkGray);
        EngineConsole.WriteLine("");
      }
    };
  }

  [BuiltIn("lists loaded assemblies with detailed info. Supports filters: contains, notcontains, equals, startswith, endswith", "[mode:value]")]
  public static Action Assemblies(string[] args)
  {
    return () =>
    {
      var assemblies = Importer.GetAssemblies();

      if (args.Length > 0)
      {
        var filtered = new List<Assembly>();

        foreach (var filterArg in args)
        {
          string mode = "contains"; // default
          string value = filterArg;

          if (filterArg.Contains(':'))
          {
            var parts = filterArg.Split(':', 2);
            mode = parts[0].ToLower();
            value = parts[1];
          }

          foreach (var asm in assemblies)
          {
            var name = asm.GetName().Name;
            bool match = mode switch
            {
              "contains" => name.Contains(value, StringComparison.CurrentCultureIgnoreCase),
              "notcontains" => !name.Contains(value, StringComparison.CurrentCultureIgnoreCase),
              "equals" => name.Equals(value, StringComparison.CurrentCultureIgnoreCase),
              "startswith" => name.StartsWith(value, StringComparison.CurrentCultureIgnoreCase),
              "endswith" => name.EndsWith(value, StringComparison.CurrentCultureIgnoreCase),
              _ => false
            };

            if (match && !filtered.Contains(asm))
              filtered.Add(asm);
          }
        }

        assemblies = filtered.ToArray();
      }

      foreach (var asm in assemblies)
      {
        var name = asm.GetName();
        var types = asm.GetTypes().Select(t => t.Name).ToArray();

        EngineConsole.WriteLine(new string('─', 50), ConsoleColor.DarkGray);
        EngineConsole.WriteLine($"Assembly: {name.Name}", ConsoleColor.Cyan);
        EngineConsole.WriteLine($"Version: {name.Version}", ConsoleColor.Green);
        EngineConsole.WriteLine($"Location: {asm.Location}", ConsoleColor.Yellow);

        if (types.Length > 0)
        {
          EngineConsole.WriteLine($"Types ({types.Length}): {string.Join(", ", types.Take(10))}" +
                                  (types.Length > 10 ? ", …" : ""), ConsoleColor.White);
        }

        EngineConsole.WriteLine(new string('─', 50), ConsoleColor.DarkGray);
        EngineConsole.WriteLine("");
      }

      if (assemblies.Length == 0)
        EngineConsole.WriteLine("No assemblies found.", ConsoleColor.Red);
    };
  }

  [BuiltIn("compiles the whole project")]
  public static Action Compile(string[] _)
  {
    return Yakoc.Compile;
  }

  [BuiltIn("copies engine's libraries to the specified path", "[path]")]
  public static Action CopyLibs(string[] args)
  {
    return () =>
    {
      if (args.Length < 0)
      {
        EngineConsole.WriteError("[!] Expected path");
        return;
      }
      try
      {
        string appDir = AppDomain.CurrentDomain.BaseDirectory;
        string currentDir = args[0];

        string[] folders = ["libs", "libraries"];

        foreach (var folder in folders)
        {
          string source = Path.Combine(appDir, folder);
          string dest = Path.Combine(currentDir, folder);

          if (!Directory.Exists(source))
          {
            EngineConsole.WriteError($"[!] Source folder not found: {source}");
            continue;
          }

          CopyDirectory(source, dest);
          EngineConsole.WriteLine($"Copied '{folder}' to '{currentDir}'", ConsoleColor.Green);
        }
      }
      catch (Exception ex)
      {
        EngineConsole.WriteError($"[!] Failed to copy folders: {ex.Message}");
      }
    };
  }

  [BuiltIn("creates a new project", "[path]")]
  public static Action Newp(string[] args)
  {
    return () =>
    {
      if (args.Length < 0)
      {
        EngineConsole.WriteError("[!] Expected path");
        return;
      }

      var path = args[0];

      if (!Directory.Exists(path)) Directory.CreateDirectory(path);

      CopyLibs([path])();
    };
  }

  [BuiltIn("shows the version")]
  public static Action Version(string[]_) => () => { EngineConsole.WriteLine(Core.Version.All); };

  [BuiltIn("shows avaible runtimes")]
  public static Action Runtimes(string[]_) => () =>
  {
    foreach (var runtime in EngineResources.GetInternalRuntimes())
    {
      EngineConsole.WriteLine($"monoe.runtime-{runtime}@{Core.Version.All} ~ {EngineResources.GetRuntime(runtime)}");
    }
  };

  private static void CopyDirectory(string sourceDir, string destinationDir)
  {
    if (!Directory.Exists(destinationDir))
      Directory.CreateDirectory(destinationDir);

    foreach (var file in Directory.GetFiles(sourceDir))
    {
      var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
      File.Copy(file, destFile, overwrite: true);
    }

    foreach (var directory in Directory.GetDirectories(sourceDir))
    {
      var destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
      CopyDirectory(directory, destDir);
    }
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