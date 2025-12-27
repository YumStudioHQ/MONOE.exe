using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using Godot;
using monoe.exe.Core.Bridge;
using monoe.exe.Core.Engine;
using Script = monoe.exe.Core.Bridge.Script;
using Timer = Godot.Timer;

namespace monoe.exe.Core;

public partial class Main : Node
{
  private static Script main = null;
  private FileSystemWatcher watcher;
  private static readonly ConcurrentQueue<Action> mainThreadQueue = new();
  private bool criticalState = false;
  private Action luaErrorHandler = null;
  private Timer cleanupTimer;
  public readonly static ConcurrentQueue<Action> GarbageCollector = [];

  public static void Emit(string @event, params object[] args)
  {
    main.Call("monoe.event.emit", [@event, .. args]);
  }

  public static object[] LCall(string method, params object[] args)
  {
    return main.Call(method, args);
  }

  public static void EnqueueOnMain(Action action) => mainThreadQueue.Enqueue(action);

  public static void InvokeOnMainThreadBlocking(Action action)
  {
    using var done = new ManualResetEventSlim(false);
    Exception error = null;

    mainThreadQueue.Enqueue(() =>
    {
      try
      {
        action();
      }
      catch (Exception e)
      {
        error = e;
      }
      finally
      {
        done.Set();
      }
    });

    done.Wait();

    if (error != null)
      throw error;
  }

  public static void Run(string code) // This Run method is only for string injections!
   => main.Run(code, false);

  private static void DumpLua(string line)
  {
    string target =
      line.Length > "$dump".Length
        ? line["$dump".Length..].TrimStart()
        : "_G";

    EngineConsole.WriteLine("");
    main.RawRun($"""
  local function dump(t)
    for key, value in pairs(t) do
      print(key, value)
    end
  end
  dump({target})
  """);
  }


  public void ShellInject(string code)
  {
    if (code == "$reload")
    {
      mainThreadQueue.Enqueue(() =>
      {
        Manager.ObjectRegistry.Clear();
        Init();
      });
    }
    else if (code.StartsWith("$dump")) DumpLua(code);
    else if (code == "$gc")
    {
      mainThreadQueue.Enqueue(() =>
      {
        while (GarbageCollector.TryDequeue(out var action))
          action();
        GC.Collect();
      });
    }
    else if (code == "$stats") EngineConsole.Print("GC.GetTotalAllocatedBytes:", GC.GetTotalAllocatedBytes(), "GC.GetTotalMemory:", GC.GetTotalMemory(true));
    else try
      {
        main.RawRun(code);
      }
      catch (Exception e)
      {
        EngineConsole.WriteError(e);
      }
  }

  public override void _EnterTree()
  {
    EngineConsole.Verbose("monoe.exe: booting...");

    /*
     * Before booting the engine, set up the ErrorHandler, and the FileSystemWatcher.
     */

    string inlineErrorHandler = "";
    foreach (var arg in OS.GetCmdlineArgs())
    {
      if (arg.StartsWith("-inline-error-handler:"))
        inlineErrorHandler = arg["-inline-error-handler:".Length..];
    }

    luaErrorHandler = () =>
    {
      criticalState = true;
      main?.Run(inlineErrorHandler, false);
    };

    if (! /* Hot reload is optional! */ OS.GetCmdlineArgs().Contains("-no-hot-reload"))
    {
      watcher = new("./")
      {
        NotifyFilter = NotifyFilters.Attributes
                     | NotifyFilters.CreationTime
                     | NotifyFilters.DirectoryName
                     | NotifyFilters.FileName
                     | NotifyFilters.LastAccess
                     | NotifyFilters.LastWrite
                     | NotifyFilters.Security
                     | NotifyFilters.Size
      };
      watcher.Changed += OnFileChanged;
      watcher.Filter = "*.lua";
      watcher.IncludeSubdirectories = true;
      watcher.EnableRaisingEvents = true;
    }

    /*
     * We also setup SceneRoot so we can use Godot through C# and Assembly without passing 
     * and keeping references to a node.
     */
    SceneRoot.SetNode(this);
  }

  public override void _Ready()
  {
    EngineConsole.Verbose("loading project");

    /*
     * At the first frame, we call the `deps()` function first (in order to request needed libraries), and then,
     * once all library loaded, we call the `main()` function.
     */
    Init();
    EngineConsole.Verbose("project ready!");

    /*
     * The free event is not fired directly after the first frame!
     */
    cleanupTimer = new()
    {
      WaitTime = 0.5,
      OneShot = false,
    };

    AddChild(cleanupTimer);

    cleanupTimer.Timeout += () =>
    {
      Emit("onfree");
    };
  }

  public override void _Process(double delta)
  {
    /*
     * Seen as `process` event in Lua, this function is designed in order to update the game.
     * If you need to update physics, use the `physics` event instead.
     */
    while (!mainThreadQueue.IsEmpty)
    {
      if (mainThreadQueue.TryDequeue(out Action action)) action();
      else EngineConsole.WriteError(new Exception("Cannot dequeue `reloadQueue`"));
    }

    if (!criticalState)
    {
      Emit("process", delta);
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    /*
     * Updates physics.
     */
    if (!criticalState)
    {
      Emit("physics", delta);
    }
  }

  public override void _ExitTree()
  {
    EngineConsole.WriteLine();
    EngineConsole.Verbose("exit requested...");
    Manager.ObjectRegistry.Clear();
    EngineConsole.Verbose("exit event fired");
    Emit("onexit");
    main.Dispose();
    watcher?.Dispose();

    while (!GarbageCollector.IsEmpty)
    {
      if (GarbageCollector.TryDequeue(out Action action))
      {
        action();
      }
      else EngineConsole.WriteError("Failled to deque an element !");
    }

    EngineConsole.Verbose("process finished");
  }

  public override void _Input(InputEvent @event)
  {
    Emit("input");
  }

  private static object[] Lsleep(object[] args)
  {
    if (args.Length > 0)
    {
      if (args[0] is long l) Thread.Sleep((int)l);
      else if (args[0] is double d) Thread.Sleep((int)d);
    }
    return [];
  }

  private void Init()
  {
    // 1. Detect the project.
    main = new("project.lua", true, luaErrorHandler);

    // 2. Load dependencies
    var libs = main.Call("deps")
                   .Where(o => o is string s && string.IsNullOrEmpty(s.Trim()))
                   .Cast<string>()
                   .ToArray();

    // 3. Load them.
    Importer.LoadAssemblies(libs);

    /* 4. Push callbacks.
     * Note: these callbacks are "visible" in monolib.lua and unique_event.lua files!
     * But you can absolutely use them without these files — They are designed only for IDEs!
     */
    main.PushCallback("monoe.import", Importer.Limport);
    main.PushCallback("monoe.call", Importer.Lcall);
    main.PushCallback("monoe.staticcall", Importer.Lstaticcall);
    main.PushCallback("monoe.wait", Lsleep);
    string injection = """
                       monoe.event.emit = monoe.event.emit or function(name)end

                       print = function(...)
                         local args = {}
                         local t = { ... }
                         for _, value in pairs(t) do
                          args[#args + 1] = tostring(value)
                         end
                         monoe.staticcall("monoe.exe.Core.Engine.EngineConsole", "Print", table.unpack(args))
                       end
                       """;
    main.Run(injection, false);

    // 5. Call main.
    Run("main = main or function() end");
    var margs = main.Call("main");

    // 6. Load scripts (They are generally requested from the main function!)
    Emit("@load");

    // 7. Finally, call ready!
    Emit("ready", margs);

    /* Quick note!
     * Generally, users love hot reloading. So, the function 'load' in monoe allows
     * hot reloading on other files than the main lua script. Idea is to subscribe a function
     * that'll load the file to two events: the @load event, and the @hot (that allows hot reloading).
     * When a hot reloading event is fired, each subscribers looks for the path: if it's their file, they'll 
     * reload the file.
     * That's also why the "ready" event is fired after loading: ready is for class (or whatever) initialization!
     *
     * Note: Your file will be in _G.module_name!
     */

    // 8. Start MONOE.exe.shell
    var thread = new Thread(Shell)
    {
      IsBackground = true
    };

    thread.Start();
  }

  private void OnFileChanged(object sender, FileSystemEventArgs e)
  {
    if (e.ChangeType != WatcherChangeTypes.Changed) return;
    EngineConsole.WriteLine($"\n> file changed {e.FullPath}", ConsoleColor.DarkGray);

    if (e.Name == "project.lua")
    {
      EngineConsole.Verbose("requested reboot...");
      mainThreadQueue.Enqueue(() =>
      {
        EngineConsole.Verbose("rebooting...");
        Manager.ObjectRegistry.Clear();
        Init();
      });
    }
    else
    {
      mainThreadQueue.Enqueue(() =>
      {
        Emit("@hot", e.FullPath);
      });
    }

    if (criticalState)
    {
      criticalState = false;
    }
  }

  private void Shell()
  {
    EngineConsole.Verbose("monoe shell — type `$reload`, `$dump`, Lua code, or `exit`");

    /* Quick notes:
     *  * The monoe shell is a way to interact with the lua code during the runtime. You can use it in order to interact 
     *    with your game, inspect elements, and get memory usage (C# side).
     *  * You can also manage errors here, or even explode your game (e.g., monoe = nil = BOOOOM)
     *  * You MAY NOT use debug.debug() in the shell, as C# takes over Lua's input request: infinite loop.
     */
    while (true)
    {
      try
      {
        EngineConsole.Write("monoe> ", ConsoleColor.Cyan);
        string line = Console.ReadLine();

        if (line == null)
          break;

        line = line.Trim();
        if (line.Length == 0)
          continue;

        if (line is "exit" or "quit") mainThreadQueue.Enqueue(QueueFree);

        InvokeOnMainThreadBlocking(() => // Blocks the current thread!
        {
          ShellInject(line);
        });
      }
      catch (Exception e)
      {
        EngineConsole.WriteError($"[Shell Error] {e.Message}");
      }
    }
  }
}
