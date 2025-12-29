using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using Godot;
using monoe.exe.Core.Bridge;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Engine.Shell;
using Script = monoe.exe.Core.Bridge.Script;
using Timer = Godot.Timer;

namespace monoe.exe.Core;

public partial class Main : Node
{
  private static Script main = null;
  private FileSystemWatcher watcher;
  private static readonly ConcurrentQueue<Action> mainThreadQueue = new();
  private static bool locked = false;
  private Action luaErrorHandler = null;
  private Timer cleanupTimer;
  public readonly static ConcurrentQueue<Action> GarbageCollector = [];
  private static Action reloadRequestAction = () => { };
  private static Action exitRequestionAction = () => { };

  public static void Emit(string @event, params object[] args)
  {
    main.Call("monoe.event.emit", [@event, .. args]);
  }

  public static object[] LCall(string method, params object[] args)
  {
    return main.Call(method, args);
  }

  public static void EnqueueOnMain(Action action) => mainThreadQueue.Enqueue(action);
  public static void RequestReload()
  {
    EngineConsole.WriteLine();
    EngineConsole.Verbose("reload requested");
    mainThreadQueue.Enqueue(reloadRequestAction);
  }
  public static void RequestLock()
  {
    EngineConsole.Verbose((locked ? "un" : "") + "lock requested...");
    mainThreadQueue.Enqueue(() =>
    {
      EngineConsole.Verbose((locked ? "un" : "") + "locking...");
      /* Note:
       *  * This won't lock directly, event after executing the action, 
       *    as other action may be queued after this one !
       */
      locked = !locked;
    });
  }

  public static void RequestExit()
  {
    EnqueueOnMain(() =>
    {
      if (AppLifetime.IsShuttingDown)
        return;

      AppLifetime.IsShuttingDown = true;
      exitRequestionAction();
    });
  }

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

  public override void _EnterTree()
  {
    if (OS.GetCmdlineArgs().Contains("-silent")) EngineConsole.IsVerbose = false;
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
      locked = true;
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

    /*
     * Also expose a static action that allow other classes to request a reload!
     */
    reloadRequestAction = () =>
    {
      EngineConsole.Verbose("reloading...");
      Manager.ObjectRegistry.Clear();
      main.Reload();
      LoadProject();
    };

    // And exit!

    exitRequestionAction = () =>
    {
      GetTree().Quit();
    };

    // Then, the shell
    if (!OS.GetCmdlineArgs().Contains("-no-shell")) Shell.Init();
  }

  public override void _Ready()
  {
    EngineConsole.Verbose("loading project");

    /*
     * At the first frame, we call the `deps()` function first (in order to request needed libraries), and then,
     * once all library loaded, we call the `main()` function.
     */
    LoadProject();
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

    if (!OS.GetCmdlineArgs().Contains("-no-shell"))
    {
      var thread = new Thread(Shell.Prompt)
      {
        IsBackground = true,
      };

      thread.Start();
    }
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

    if (!locked)
    {
      Emit("process", delta);
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    /*
     * Updates physics.
     */
    if (!locked)
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

  private void LoadProject()
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
    main.PushCallback("monoe.shell", Shell.Prompt);
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
        LoadProject();
      });
    }
    else
    {
      mainThreadQueue.Enqueue(() =>
      {
        Emit("@hot", e.FullPath);
      });
    }

    if (locked)
    {
      locked = false;
    }
  }
}
