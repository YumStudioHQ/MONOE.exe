using System;
using System.Collections.Generic;
using System.IO;

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

  private static RuntimeResource GetMacOSRuntime()
  {
    string name = "osx";
    string path = GetResourceDir("runtimes", name);
    string resources = "monoe.exe.app/Contents/Resources";
    return new(name, path, resources);
  }

  static EngineResources()
  {
    List<RuntimeResource> runtimes = [
      GetMacOSRuntime()
    ];

    string[] runtimeNames = ["lin64", "lin32", "linarm64", "win64", "win32", "winarm64"];

    foreach (string name in runtimeNames)
    {
      string path = GetResourceDir("runtimes", name);
      runtimes.Add(new(name, path, ""));
    }

    runtimeResources = [..runtimes];
  }

  public static RuntimeResource[] GetRuntimes()
   => runtimeResources;
}