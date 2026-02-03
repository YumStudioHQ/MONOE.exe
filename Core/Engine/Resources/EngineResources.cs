using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace monoe.exe.Core.Engine.Resources;

public static class EngineResources
{
  private static MonoeRuntimeInfo[] runtimeResources = [];

  public static string GetResourceDir()
   => Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;

  public static string GetRuntimeResourceDir()
   => Application.IsDevMode ? Path.GetFullPath("./res/") : GetResourceDir();

  public static string GetResourceDir(params string[] paths)
   => Path.Join([GetResourceDir(), .. paths]);

  private static string FormatPathForLua(string path)
  {
    if (path.Trim() == "") return "";

    path = path.Replace("@PWD", Application.PWD)
               .Replace('\\', '/');

    if (path.EndsWith('/'))
      path = path[..^1];

    return $"';{path}/?.lua'";
  }

  public static string LuaLibrariesFmt()
  {
    var query = "''";

    foreach (var libdel in Application.Libraries)
      query += ".." + FormatPathForLua(libdel);

    return query;
  }


  private static void Init()
  {
    List<MonoeRuntimeInfo> runtimes = [];
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

    runtimeResources = [.. runtimes];
  }

  static EngineResources()
  {
    EngineConsole.Verbose($"before boot: {Directory.GetCurrentDirectory()} (in .ctors)");

    try
    {
      Init();
    }
    #if !DEBUG
    catch (Exception e)
    {
      EngineConsole.WriteError(e);
    }
    #else
    catch (Exception) {}
    #endif
  }

  public static MonoeRuntimeInfo[] GetRuntimes()
   => runtimeResources;
}