using System.Collections.Generic;
using System.IO;

namespace monoe.exe.Core.Export.io;

public static class Filesystem
{
  public static object[] GetFilesFrom(object[] objects)
  {
    var files = new List<string>();
    foreach (var o in objects)
    {
      if (o is string dir && Directory.Exists(dir))
      {
        files.AddRange(Directory.EnumerateFiles(dir));
      }
    }
    return files.ToArray();
  }

  public static object[] GetFoldersFrom(object[] objects)
  {
    var directories = new List<string>();
    foreach (var o in objects)
    {
      if (o is string dir && Directory.Exists(dir))
      {
        directories.AddRange(Directory.EnumerateDirectories(dir));
      }
    }
    return directories.ToArray();
  }

  public static object[] GetFilesRecursive(object[] directories)
  {
    var files = new List<string>();
    foreach (var o in directories)
    {
      if (o is string dir && Directory.Exists(dir))
      {
        files.AddRange(Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories));
      }
    }
    return files.ToArray();
  }

  public static object[] Absolute(object[] objects)
  {
    var paths = new List<string>();
    foreach (var o in objects)
    {
      if (o is string path && (File.Exists(path) || Directory.Exists(path)))
      {
        paths.Add(Path.GetFullPath(path));
      }
    }
    return paths.ToArray();
  }

  public static object[] FileName(object[] objects)
  {
    var names = new List<string>();
    foreach (var o in objects)
    {
      if (o is string path && (File.Exists(path) || Directory.Exists(path)))
      {
        names.Add(Path.GetFileName(path));
      }
    }
    return names.ToArray();
  }
}
