using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Godot;

namespace monoe.exe.Core;

public partial class Main : Node
{
  private Script main = null;
  private FileSystemWatcher watcher;
  private readonly ConcurrentQueue<Action> reloadQueue = new();
  private Reflector reflector;
  private bool criticalState = false;
  private Action eventHandler = null;

  public override void _Ready()
  {
    eventHandler = () =>
    {
      criticalState = true;
    };

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

    Init();
  }

  public override void _Process(double delta)
  {
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
    if (!criticalState)
      main.Call("physics", delta);
  }

  public override void _ExitTree()
  {
    CloseStates();
    watcher.Dispose();
  }

  private void Init()
  {
    main = new("project.lua", true, eventHandler);

    var libs = main.Call("deps")
                   .Where(o => o is string s && string.IsNullOrEmpty(s.Trim()))
                   .Cast<string>()
                   .ToArray();

    reflector = new(libs);

    main.PushCallback("monoe.import", reflector.Limport);
    main.PushCallback("monoe.call", reflector.Lcall);
    main.PushCallback("monoe.staticcall", reflector.Lstaticcall);
    main.Call("main");
  }

  private void CloseStates()
  {
    main.Call("exit");
  }

  private void OnFileChanged(object sender, FileSystemEventArgs e)
  {
    if (e.ChangeType != WatcherChangeTypes.Changed) return;

    reloadQueue.Enqueue(() =>
    {
      CloseStates();
      Init();
    });

    if (criticalState)
    {
      criticalState = false;
      GD.Print("\n>>> reloaded\n");
    }
  }
}