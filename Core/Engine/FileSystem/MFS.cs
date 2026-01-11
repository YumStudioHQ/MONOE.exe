using System;
using System.IO;

namespace monoe.exe.Core.Engine.FileSystem;

public static class MonoeFileSystem
{
  public readonly static string MonoeDir = ".monoe";

  public static string Local(params string[] args)
   => Path.Join([MonoeDir, ..args]);

  public static void CopyFolder(string source, string destination)
  {
    if (!Directory.Exists(source))
    {
      EngineConsole.WriteLine($"[Warning] Source folder '{source}' does not exist.", ConsoleColor.Yellow);
      return;
    }

    var sourceFull = Path.GetFullPath(source);
    var destFull = Path.GetFullPath(destination);

    if (destFull.StartsWith(sourceFull, StringComparison.OrdinalIgnoreCase))
      throw new InvalidOperationException($"Refusing to copy '{source}' into its own subdirectory.");
    Directory.CreateDirectory(destination);

    foreach (var file in Directory.GetFiles(source))
    {
      File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
    }
    foreach (var dir in Directory.GetDirectories(source))
    {
      CopyFolder(dir, Path.Combine(destination, Path.GetFileName(dir)));
    }
  }
}