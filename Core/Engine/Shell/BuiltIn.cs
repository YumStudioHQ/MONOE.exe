using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using monoe.exe.Core.Bridge;
using monoe.exe.Core.Engine.Compiler;
using monoe.exe.Core.Engine.Resources;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Engine.Shell;

[ShellCommandDelegate]
public static class BuiltIns
{
  [ShellCommand("dump", help: "Dumps a Lua table", args: ["<table> (default: _G)"])]
  public static void Dump(string[] args)
  {
    string code = """
                      local function dump(t)
                          for key, value in pairs(t) do
                              print(key, value)
                          end
                      end
                      """;

    if (args.Length == 0)
      code += "dump(_G);";
    else
      foreach (var table in args)
        code += $"dump({table});";

    Base.MainBase.Run(code);
  }

  [ShellCommand("reload", help: "Reloads the whole project")]
  public static void Reload(string[] _) => Base.MainBase.RequestReload();

  [ShellCommand("lock", help: "Locks / unlocks the engine's main loop. Commands can still be executed.")]
  public static void Lock(string[] _) => Base.MainBase.RequestLock();

  [ShellCommand("inspect", help: "Inspects a Lua value", args: ["<expr>"])]
  public static void Inspect(string[] args)
  {
    if (args.Length == 0) return;

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

    Base.MainBase.Run(code);
  }

  [ShellCommand("sleep", help: "Blocks the shell thread for N milliseconds", args: ["ms"])]
  public static void Sleep(string[] args)
  {
    if (args.Length == 0) return;
    if (int.TryParse(args[0], out int ms))
      Thread.Sleep(ms);
  }

  [ShellCommand("stats", help: "Prints GC and memory statistics")]
  public static void Stats(string[] _) =>
      EngineConsole.Print("Allocated:", GC.GetTotalAllocatedBytes(), "Total:", GC.GetTotalMemory(false));

  [ShellCommand("help", help: "Lists all shell commands or details one command", args: ["[command]"])]
  public static void Help(string[] args)
  {
    var commands =
      EngineAssembly.GetTypes()
        .Where(t => t.GetCustomAttribute<ShellCommandDelegateAttribute>() != null)
        .SelectMany(t =>
          t.GetMethods(BindingFlags.Public | BindingFlags.Static)
           .Select(m => (Method: m, Attr: m.GetCustomAttribute<ShellCommandAttribute>()))
           .Where(x => x.Attr != null))
        .OrderBy(c => c.Attr.Name)
        .ToList();

    if (args.Length == 0)
    {
      const int nameWidth = 12;
      const int argsWidth = 20;

      EngineConsole.WriteLine("Available Commands", ConsoleColor.Cyan);
      EngineConsole.WriteLine(new string('─', 60), ConsoleColor.DarkGray);

      foreach (var (_, attr) in commands)
      {
        var name = attr.Name.PadRight(nameWidth);
        var argsText = string.Join(" ", attr.Arguments ?? Array.Empty<string>())
                            .PadRight(argsWidth);

        EngineConsole.Write(name, ConsoleColor.Cyan);
        EngineConsole.WriteLine(
          $" {argsText} {attr.Help}"
        );
      }

      EngineConsole.WriteLine(new string('─', 60), ConsoleColor.DarkGray);
      EngineConsole.WriteLine("Type `help <command>` for details", ConsoleColor.DarkBlue);
      return;
    }

    var command = commands.FirstOrDefault(c =>
      c.Attr.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase));

    if (command.Method == null)
    {
      EngineConsole.WriteError($"Unknown command '{args[0]}'");
      return;
    }

    var info = command.Attr;

    EngineConsole.WriteLine(info.Name, ConsoleColor.Cyan);
    EngineConsole.WriteLine(new string('─', 40), ConsoleColor.DarkGray);
    EngineConsole.WriteLine(info.Help, ConsoleColor.White);

    if (info.Arguments?.Length > 0)
    {
      EngineConsole.WriteLine("\nUsage:", ConsoleColor.Green);
      EngineConsole.WriteLine(
        $"  {info.Name} {string.Join(" ", info.Arguments)}",
        ConsoleColor.Gray
      );
    }
  }

  [ShellCommand("clear", help: "Clears the console")]
  public static void Clear(string[] _) => Console.Clear();

  [ShellCommand("exit", help: "Quits the engine")]
  public static void Exit(string[] _) => Base.MainBase.RequestExit();

  [ShellCommand("emit", help: "Emits an event with given arguments", args: ["[eventName]", "[args...]"])]
  public static void Emit(string[] args)
  {
    if (args.Length == 0)
    {
      EngineConsole.WriteError("emit requires an event name");
      return;
    }

    var eventName = args[0];
    var parsed = args.Skip(1).Select(ParseArg).ToArray();
    Base.MainBase.Emit(eventName, parsed);
  }

  [ShellCommand("object", help: "Shows detailed info about a C# object based on its UID", args: ["[IDs...]"])]
  public static void Object(string[] args)
  {
    foreach (var strUid in args)
    {
      if (!long.TryParse(strUid, out long uid))
      {
        EngineConsole.WriteError($"Expected integer UID, got '{strUid}'");
        continue;
      }

      if (!ObjectRegistry.TryGet(uid, out object o))
      {
        EngineConsole.WriteError($"No object found for UID {uid}");
        continue;
      }

      var type = o.GetType();
      var assembly = type.Assembly.GetName().Name;
      var baseType = type.BaseType?.FullName ?? "none";
      var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                           .Select(p => $"{p.Name} ({p.PropertyType.Name})");
      var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                       .Select(f => $"{f.Name} ({f.FieldType.Name})");
      var interfaces = type.GetInterfaces().Select(i => i.Name);

      EngineConsole.WriteLine(new string('─', 40), ConsoleColor.DarkGray);
      EngineConsole.WriteLine($"UID: {uid}", ConsoleColor.Cyan);
      EngineConsole.WriteLine($"Type: {type.FullName}", ConsoleColor.Green);
      EngineConsole.WriteLine($"Base: {baseType}", ConsoleColor.Yellow);
      EngineConsole.WriteLine($"Assembly: {assembly}", ConsoleColor.Magenta);

      if (interfaces.Any())
        EngineConsole.WriteLine($"Implements: {string.Join(", ", interfaces)}", ConsoleColor.Blue);

      if (properties.Any())
        EngineConsole.WriteLine($"Properties: {string.Join(", ", properties)}", ConsoleColor.White);

      if (fields.Any())
        EngineConsole.WriteLine($"Fields: {string.Join(", ", fields)}", ConsoleColor.Gray);

      EngineConsole.WriteLine(new string('─', 40), ConsoleColor.DarkGray);
      EngineConsole.WriteLine("");
    }
  }

  [ShellCommand("assemblies", help: "Lists loaded assemblies with filtering", args: ["[mode:value]"])]
  public static void Assemblies(string[] args)
  {
    var assemblies = Importer.GetAssemblies().ToList();

    if (args.Length > 0)
    {
      assemblies = assemblies.Where(asm =>
      {
        foreach (var filterArg in args)
        {
          var parts = filterArg.Split(':', 2);
          string mode = parts.Length > 1 ? parts[0].ToLower() : "contains";
          string value = parts.Length > 1 ? parts[1] : parts[0];

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

          if (match) return true;
        }
        return false;
      }).ToList();
    }

    if (!assemblies.Any())
    {
      EngineConsole.WriteLine("No assemblies found.", ConsoleColor.Red);
      return;
    }

    foreach (var asm in assemblies)
    {
      var name = asm.GetName();
      var types = asm.GetTypes().Select(t => t.Name).ToArray();

      EngineConsole.WriteLine(new string('─', 50), ConsoleColor.DarkGray);
      EngineConsole.WriteLine($"Assembly: {name.Name}", ConsoleColor.Cyan);
      EngineConsole.WriteLine($"Version: {name.Version}", ConsoleColor.Green);
      EngineConsole.WriteLine($"Location: {asm.Location}", ConsoleColor.Yellow);
      EngineConsole.WriteLine($"Types ({types.Length}): {string.Join(", ", types.Take(10))}{(types.Length > 10 ? ", …" : "")}",
                              ConsoleColor.White);
      EngineConsole.WriteLine(new string('─', 50), ConsoleColor.DarkGray);
      EngineConsole.WriteLine("");
    }
  }

  [ShellCommand("compile", help: "Compiles the whole project")]
  public static void Compile(string[] _) => Monoec.Compile();

  [ShellCommand("copylibs", help: "Copies engine's libraries to the specified path", args: ["[path]"])]
  public static void CopyLibs(string[] args)
  {
    if (args.Length == 0)
    {
      EngineConsole.WriteError("Expected path");
      return;
    }

    string appDir = AppDomain.CurrentDomain.BaseDirectory;
    string currentDir = args[0];
    string[] folders = ["libs", "libraries"];

    foreach (var folder in folders)
    {
      string source = Path.Combine(appDir, folder);
      string dest = Path.Combine(currentDir, folder);

      if (!Directory.Exists(source))
      {
        EngineConsole.WriteError($"Source folder not found: {source}");
        continue;
      }

      CopyDirectory(source, dest);
      EngineConsole.WriteLine($"Copied '{folder}' to '{currentDir}'", ConsoleColor.Green);
    }
  }

  [ShellCommand("newp", help: "Creates a new project", args: ["[path]"])]
  public static void Newp(string[] args)
  {
    if (args.Length == 0)
    {
      EngineConsole.WriteError("Expected path");
      return;
    }

    var path = args[0];
    if (!Directory.Exists(path))
      Directory.CreateDirectory(path);

    CopyLibs([path]);
  }

  [ShellCommand("version", help: "Shows the version")]
  public static void Version(string[] _) => EngineConsole.WriteLine(Core.Version.All);

  [ShellCommand("runtimes", help: "Shows available runtimes")]
  public static void Runtimes(string[] _)
  {
    foreach (var runtime in EngineResources.GetRuntimes())
    {
      EngineConsole.WriteLine($"monoe.runtime-{runtime.Name}@{Core.Version.All} ~ {runtime.Path}");
    }
  }

  private static void CopyDirectory(string sourceDir, string destinationDir)
  {
    Directory.CreateDirectory(destinationDir);

    foreach (var file in Directory.GetFiles(sourceDir))
      File.Copy(file, Path.Combine(destinationDir, Path.GetFileName(file)), overwrite: true);

    foreach (var dir in Directory.GetDirectories(sourceDir))
      CopyDirectory(dir, Path.Combine(destinationDir, Path.GetFileName(dir)));
  }

  private static object ParseArg(string arg)
  {
    if (string.IsNullOrEmpty(arg)) return "";

    if ((arg.StartsWith('"') && arg.EndsWith('"')) || (arg.StartsWith('\'') && arg.EndsWith('\'')))
      return arg[1..^1];

    if (bool.TryParse(arg, out var b)) return b;
    if (long.TryParse(arg, out var l)) return l;
    if (double.TryParse(arg, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var d)) return d;
    if (arg.Trim() is "nil" or "null") return null;

    return arg;
  }
}
