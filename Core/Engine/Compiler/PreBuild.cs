using System;
using System.IO;

namespace monoe.exe.Core.Engine.Compiler
{
  public static class PreBuild
  {
    public static void PrepareBuild()
    {
      string buildDir = "build";

      if (!Directory.Exists(buildDir))
        Directory.CreateDirectory(buildDir);
      CopyFolder("libraries", Path.Combine(buildDir, "libraries"));
      CopyFolder("libs", Path.Combine(buildDir, "libs"));
      CopyFolder("res", Path.Combine(buildDir, "res"));
    }

    public static void CopyFolder(string source, string destination)
    {
      if (!Directory.Exists(source))
      {
        EngineConsole.WriteLine($"[Warning] Source folder '{source}' does not exist.", ConsoleColor.Yellow);
        return;
      }

      Directory.CreateDirectory(destination);

      foreach (var file in Directory.GetFiles(source))
      {
        var destFile = Path.Combine(destination, Path.GetFileName(file));
        File.Copy(file, destFile, true);
      }

      foreach (var dir in Directory.GetDirectories(source))
      {
        var destDir = Path.Combine(destination, Path.GetFileName(dir));
        CopyFolder(dir, destDir);
      }

      EngineConsole.WriteLine($"Copied '{source}' → '{destination}'", ConsoleColor.Yellow);
    }
  }
}
