using System.Collections.Generic;
using System.IO;

namespace monoe.exe.Core.Bridge.io;

public static class Filesystem
{
  public static string[] GetFilesFrom(string[] args)
  {
    var files = new List<string>();
    foreach (var dir in args)
    {
      if (Directory.Exists(dir))
      {
        files.AddRange(Directory.EnumerateFiles(dir));
      }
    }
    return [.. files];
  }

  public static string[] GetFoldersFrom(string[] args)
  {
    var directories = new List<string>();
    foreach (var dir in args)
    {
      if (Directory.Exists(dir))
      {
        directories.AddRange(Directory.EnumerateDirectories(dir));
      }
    }
    return [.. directories];
  }

  public static string[] GetFilesRecursive(string[] directories)
  {
    var files = new List<string>();
    foreach (var dir in directories)
    {
      if (Directory.Exists(dir))
      {
        files.AddRange(Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories));
      }
    }
    return [.. files];
  }

  public static string[] Absolute(string[] args)
  {
    var paths = new List<string>();
    foreach (var path in args)
    {
      if (File.Exists(path) || Directory.Exists(path))
      {
        paths.Add(Path.GetFullPath(path));
      }
    }
    return [.. paths];
  }

  public static string[] FileName(string[] args)
  {
    var names = new List<string>();
    foreach (var path in args)
    {
      if (File.Exists(path) || Directory.Exists(path))
      {
        names.Add(Path.GetFileName(path));
      }
    }
    return [.. names];
  }

  public static object[] Exists(object[] objects)
  {
    foreach (var o in objects)
      if (o is string path) return [Path.Exists(path)];
    return [false];
  }

  public static object[] IsFile(object[] objects)
  {
    foreach (var o in objects)
      if (o is string path) return [File.Exists(path)];
    return [false];
  }
}
