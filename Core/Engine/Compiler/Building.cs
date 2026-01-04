using System;
using System.IO;
using System.IO.Compression;
using monoe.exe.Core.Engine.Resources;

namespace monoe.exe.Core.Engine.Compiler;

public static class Building
{
  private readonly static string BuildDir = Path.Join(".monoe", "build");
  private readonly static string AssembliesDir = Path.Join(BuildDir, "assemblies");
  private readonly static string MonoelibDir = Path.Join(BuildDir, "monoelib");
  private readonly static string GameResourcesDir = Path.Join(BuildDir, "res");
  private readonly static string GameAssemblyFileName = "monoe.game.dll";
  private readonly static string GameAssemblyOutFolder = Path.Join(BuildDir, "assemblies");
  public readonly static string GameAssemblyOutPath = Path.Join(GameAssemblyOutFolder, GameAssemblyFileName);

  public static string GetBuildDir() => Path.GetFullPath(BuildDir);
  public static string GetAssembliesDir() => Path.GetFullPath(AssembliesDir);

  public static void PrepareBuild()
  {
    Directory.CreateDirectory(AssembliesDir);
    CopyFolder(EngineResources.GetResourceDir("monoelib"), MonoelibDir); 
    CopyFolder("res", GameResourcesDir);
  }

  public static void BuildReleases()
  {
    var runtimes = EngineResources.GetRuntimes();

    foreach (var runtime in runtimes)
    {
      EngineConsole.Verbose($"compiling for platform {runtime.Name}");
      var @out = Path.Join(BuildDir, runtime.Name);
      var runtimeRelRes = Path.Join(@out, runtime.ResourcesRelative);
      
      CopyFolder(runtime.Path, @out);
      CopyFolder(MonoelibDir, Path.Join(runtimeRelRes, "monoelib"));
      CopyFolder(GameResourcesDir, Path.Join(runtimeRelRes, "res"));
      CopyFolder(GameAssemblyOutFolder, runtimeRelRes);

      Directory.CreateDirectory("out");

      var zipPath = Path.Combine("out", $"{runtime.Name}.zip");
      if (File.Exists(zipPath))
        File.Delete(zipPath);

      ZipFile.CreateFromDirectory(@out, zipPath);
      EngineConsole.WriteLine($"task {runtime.Name}: ok");
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