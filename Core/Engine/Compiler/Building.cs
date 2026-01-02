using System;
using System.IO;
using System.IO.Compression;

namespace monoe.exe.Core.Engine.Compiler;

public static class Building
{
  private readonly static string BuildDir = Path.Join(".monoe", "build");
  private readonly static string AssembliesDir = Path.Join(BuildDir, "assemblies");

  public static string GetBuildDir() => Path.GetFullPath(BuildDir);

  public static void PrepareBuild()
  {
    Directory.CreateDirectory(AssembliesDir);
    CopyFolder(EngineResources.GetResourceDir("monoelib"), Path.Combine(AssembliesDir, "monoelib")); 
    CopyFolder("res", Path.Combine(AssembliesDir, "res"));
  }

  public static void BuildReleases()
  {
    var runtimes = EngineResources.GetInternalRuntimes();

    foreach (var runtime in runtimes)
    {
      EngineConsole.Verbose($"compiling for platform {runtime}");
      var @out = Path.Join(BuildDir, runtime);
      CopyFolder(AssembliesDir, @out);
      CopyFolder(EngineResources.GetRuntime(runtime), @out);
      
      Directory.CreateDirectory("out");

      var zipPath = Path.Combine("out", $"{runtime}.zip");
      if (File.Exists(zipPath))
        File.Delete(zipPath);

      ZipFile.CreateFromDirectory(@out, zipPath);
      EngineConsole.WriteLine($"task {runtime}: ok");
    }
  }

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