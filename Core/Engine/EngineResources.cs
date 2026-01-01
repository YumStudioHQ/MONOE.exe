using System;
using System.IO;

namespace monoe.exe.Core.Engine;

public static class EngineResources
{
  public static string GetResourceDir()
   => Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName; 
  
  public static string GetResourceDir(params string[] paths)
   => Path.Join([GetResourceDir(), ..paths]);

  public static string LuaLibrariesFmt()
  { 
    var path = GetResourceDir();
    path = !path.EndsWith('/') ? path : path[..(path.Length - 1)];
    return $"';{path}/?.lua'";
  }

  public static string[] GetInternalRuntimes() => [
    "osx", "linx64", "linarm64", "lin32",
    "winx64", "winarm64", "win32",
  ];

  public static string GetRuntime(string name)
    => GetResourceDir("runtime", name);
}