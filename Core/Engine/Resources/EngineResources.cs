using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace monoe.exe.Core.Engine.Resources;

public static class EngineResources
{
  private readonly static RuntimeResource[] runtimeResources = [];

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

  static EngineResources()
  {
    List<RuntimeResource> runtimes = [];
    var dir = GetResourceDir("runtimes");
    var files = Directory.GetFiles(dir)
                         .Where(file => file.EndsWith(".zip"))
                         .ToArray();
    
    foreach (var file in files)
    {
      var resrel = "";
      if (file.Contains("osx"))
      {
        resrel = "Contents/Resources/";
      }

      runtimes.Add(new(file.Replace(".zip", ""), Path.Join(dir, Path.GetFileName(file)), resrel));
    }

    runtimeResources = [..runtimes];
  }

  public static RuntimeResource[] GetRuntimes()
   => runtimeResources;
}