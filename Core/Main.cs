using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Godot;
using monoe.exe.Core.Bridge;
using Script = monoe.exe.Core.Bridge.Script;

namespace monoe.exe.Core;

public partial class Main : Node
{
  private static Script main = null;
  private FileSystemWatcher watcher;
  private readonly ConcurrentQueue<Action> reloadQueue = new();
  private bool criticalState = false;
  private Action luaErrorHandler = null;
  private Timer cleanupTimer;
  public readonly static ConcurrentQueue<Action> GarbageCollector = [];

  public static void Emit(string @event, params object[] args)
  {
    main.Call("monoe.event.emit", [@event, ..args]);
  }

  public static object[] LCall(string method, params object[] args)
  {
    return main.Call(method, args);
  }

  public static void Run(string code) // This Run method is only for string injections!
   => main.Run(code, false);
  

  public override void _EnterTree()
  {
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
    /*
     * At the first frame, we call the `deps()` function first (in order to request needed libraries), and then,
     * once all library loaded, we call the `main()` function.
     */
    Init();
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
      main.Call("monoe.event.emit", "onfree");
    };
  }

  public override void _Process(double delta)
  {
    /*
     * Seen as `process` function in Lua, this function is designed in order to update the game.
     * If you need to update physics, use the `physics()` function instead.
     */
    if (!criticalState)
    {
      while (!reloadQueue.IsEmpty)
      {
        if (reloadQueue.TryDequeue(out Action action)) action();
        else GD.PrintErr(new Exception("Cannot dequeue `reloadQueue`"));
      }

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
    Manager.ObjectRegistry.Clear();
    Emit("monoe.event.emit", "onexit");
    main.Dispose();
    watcher?.Dispose();
    while (!GarbageCollector.IsEmpty)
    {
      if (GarbageCollector.TryDequeue(out Action action))
      {
        action();
      } else GD.Print("<err>: Failled to deque an element !");
    }
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
    string injection = """
                       function ready()end
                       function process()end
                       function physics()end
                       function exit()end
                       monoe.event.emit = monoe.event.emit or function(name)end
                       """;
    main.Run(injection, false);

    // 5. Call main.
    Run("main = main or function() end");
    Emit("ready", main.Call("main"));
  }

  private void OnFileChanged(object sender, FileSystemEventArgs e)
  {
    if (e.ChangeType != WatcherChangeTypes.Changed) return;

    if (e.Name == "project.lua")
    {
      reloadQueue.Enqueue(() =>
      {
        Manager.ObjectRegistry.Clear();
        Init();
      });
    }
    else
    {
      reloadQueue.Enqueue(() =>
      {
        main.Run("project.lua", true);
        main.Reload();
      });
    }


    if (criticalState)
    {
      criticalState = false;
      GD.Print("\n>>> reloaded\n");
    }
  }
}