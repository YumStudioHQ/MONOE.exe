using System.IO;
using Godot;
using monoe.exe.Core.Base;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types.LibSys;

public class FSWatcher : ManagedObject
{
  private readonly FileSystemWatcher watcher;
  private readonly string eventID = "";

  public FSWatcher(string path, string filter)
  {
    watcher = new(path, filter)
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

    watcher.Changed += OnChanged;
    watcher.Created += OnCreated;
    watcher.Deleted += OnDeleted;
    watcher.Renamed += OnRenamed;
    watcher.Error += OnError;

    watcher.IncludeSubdirectories = true;
    watcher.EnableRaisingEvents = true;
    eventID = $"@fsw#{System.Guid.NewGuid()}";
  }

  public void SetPath(string path) => watcher.Path = path;
  public void SetFilter(string filter) => watcher.Filter = filter;
  public void IncludeSubdirectories(bool doesIt) => watcher.IncludeSubdirectories = doesIt;
  public string GetEventBaseName() => eventID;

  private void OnChanged(object sender, FileSystemEventArgs e)
  {
    if (e.ChangeType != WatcherChangeTypes.Changed)
    {
      return;
    }

    MainBase.EnqueueOnMain(() =>
    {
      MainBase.Emit($"{eventID}_changed", Path.GetFullPath(e.FullPath));
    });
  }

  private  void OnCreated(object sender, FileSystemEventArgs e)
  {
    MainBase.EnqueueOnMain(() =>
    {
      MainBase.Emit($"{eventID}_created", Path.GetFullPath(e.FullPath));
    });
  }

  private void OnDeleted(object sender, FileSystemEventArgs e)
  {
    MainBase.EnqueueOnMain(() =>
    {
      MainBase.Emit($"{eventID}_deleted", Path.GetFullPath(e.FullPath));
    });
  }

  private void OnRenamed(object sender, RenamedEventArgs e)
  {
    MainBase.EnqueueOnMain(() =>
    {
      MainBase.Emit($"{eventID}_renamed", Path.GetFullPath(e.FullPath));
    });
  }

  private static void OnError(object sender, ErrorEventArgs e)
   => EngineConsole.WriteError(e.GetException());

  protected override void _Free()
  {
    watcher?.Dispose();
  }
}