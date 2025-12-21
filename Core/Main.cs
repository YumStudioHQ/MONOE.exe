using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

namespace monoe.exe.Core;

public partial class Main : Node
{
  private Dictionary<string, Script> scripts = [];
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

      foreach (var script in scripts) script.Value.Call("process", delta);
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!criticalState)
      foreach (var script in scripts) script.Value.Call("physics", delta);
  }

  public override void _ExitTree()
  {
    CloseStates();
    watcher.Dispose();
  }

  private void Init()
  {
    using Script main = new("project.lua", true, eventHandler);

    var @out = main.Call("main").ToArray();

    var libs = main.Call("deps")
                   .Where(o => o is string s && string.IsNullOrEmpty(s.Trim()))
                   .Cast<string>()
                   .ToArray();

    reflector = new(libs);

    foreach (var v in @out) if (v is string s)
    {
      var path = Path.GetFullPath(s);
      var script = new Script(path, true, eventHandler);
      script.PushCallback("monoe.import", reflector.Limport);
      script.PushCallback("monoe.call", reflector.Lcall);
      scripts[path] = script;
    }

    foreach (var script in scripts) script.Value.Call("ready");
  }

  private void CloseStates()
  {
    foreach (var script in scripts)
    {
      script.Value.Call("exit");
      script.Value.Dispose();
    }

    scripts = [];
  }

  private void OnFileChanged(object sender, FileSystemEventArgs e)
  {
    if (e.ChangeType != WatcherChangeTypes.Changed) return;
    else if (scripts.TryGetValue(Path.GetFullPath(e.FullPath), out Script script))
    {
      reloadQueue.Enqueue(() =>
      {
        script.Call("exit");
        script.Reload();
        script.Call("ready");
      });
    }
    else if (Path.GetFileName(e.FullPath) == "project.lua")
    {
      reloadQueue.Enqueue(() =>
      {
        CloseStates();
        Init();
      });
    }

    if (criticalState)
    {
      criticalState = false;
      GD.Print("\n>>> reloaded\n");
    }
  }
}