using System.IO;
using System.IO.Compression;
using monoe.exe.Core.Engine.FileSystem;
using monoe.exe.Core.Engine.Resources;

namespace monoe.exe.Core.Engine.Compiler;

public class Monoec
{
  private static void BuildBinaries(string localResources)
  {
    foreach (var runtime in EngineResources.GetRuntimes())
    {
      var unzippedRuntime = $"out/{runtime.Name}";
      ZipFile.ExtractToDirectory(runtime.Path, unzippedRuntime);
      var respath = Path.Join(unzippedRuntime, runtime.ResourcesRelative, "res");
      var libspath = Path.Join(unzippedRuntime, runtime.ResourcesRelative, "monoelib");
      MonoeFileSystem.CopyFolder(localResources, respath);
      MonoeFileSystem.CopyFolder(EngineResources.GetResourceDir("monoe"), libspath);
      if (runtime.Name == "osx")
      {
        EngineConsole.WriteLine($"[!] You may sign the app with this command on macOS: odesign --force --deep --sign - MAC_APP.app in order to make your application runnable on macOS (it won't load as-is, as the engine modifies an already existing app bundle)", System.ConsoleColor.Yellow);
      }
    }
  }

  public static void Compile()
  {
    BuildBinaries("res");
  }
}