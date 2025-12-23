using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Godot;

using Script = monoe.exe.Core.Bridge.Script;

namespace monoe.exe.Core;

public partial class Main : Node
{
  private Script main = null;
  private FileSystemWatcher watcher;
  private readonly ConcurrentQueue<Action> reloadQueue = new();
  private bool criticalState = false;
  private Action luaErrorHandler = null;

  public override void _EnterTree()
  {
    /*
     * Before booting the engine, set up the ErrorHandler, and the FileSystemWatcher.
     */

    string inlineErrorHandler = "";
    foreach (var arg in OS.GetCmdlineArgs())
    {
      if (arg.StartsWith("-inline-error-handler"))
        inlineErrorHandler = arg["-inline-error-handler".Length..];
    }

    luaErrorHandler = () =>
    {
      criticalState = true;
      main.Run(inlineErrorHandler, false);
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
  }

  public override void _Ready()
  {
    /*
     * At the first frame, we call the `deps()` function first (in order to request needed libraries), and then,
     * once all library loaded, we call the `main()` function.
     */
    Init();
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
        else GD.PrintErr(new FieldAccessException("Cannot dequeue `reloadQueue`"));
      }

      main.Call("process", delta);
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    /*
     * Updates physics, but also fire the `onfree` event.
     */
    if (!criticalState)
    {
      main.Call("physics", delta);
      main.Call("monoe.emit", "onfree");
    }
  }

  public override void _ExitTree()
  {
    main.Call("exit");
    main.Dispose();
    watcher?.Dispose();
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
    Bridge.Importer.LoadAssemblies(libs);

    /* 4. Push callbacks.
     * Note: these callbacks are "visible" in monolib.lua and unique_event.lua files!
     * But you can absolutely use them without these files — They are designed only for IDEs!
     */
    main.PushCallback("monoe.import", Bridge.Importer.Limport);
    main.PushCallback("monoe.call", Bridge.Importer.Lcall);
    main.PushCallback("monoe.staticcall", Bridge.Importer.Lstaticcall);
    main.Run("monoe.emit = monoe.emit or function(name)end", false); // Ugly injection...

    // 5. Call main.
    main.Call("main");
  }

  private void OnFileChanged(object sender, FileSystemEventArgs e)
  {
    if (e.ChangeType != WatcherChangeTypes.Changed) return;

    reloadQueue.Enqueue(() =>
    {
      main.Call("exit");
      Init();
    });

    if (criticalState)
    {
      criticalState = false;
      GD.Print("\n>>> reloaded\n");
    }
  }
}