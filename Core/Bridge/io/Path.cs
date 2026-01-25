using System.IO;
using monoe.exe.Core.Engine.Resources;

namespace monoe.exe.Core.Bridge.io;

public static class PathLib
{
  private const string RES_PREFIX = "@res:/";

  public static string FullPath(string of)
  {
    if (of.StartsWith(RES_PREFIX))
    {
      var resDir = EngineResources.GetRuntimeResourceDir();
      return Path.GetFullPath(Path.Join(resDir, of[RES_PREFIX.Length..].TrimStart('/', '\\')));
    }

    return Path.GetFullPath(of);
  }
  
  public static void CopyDirectory(string sourcePath, string targetPath)
  {
    foreach (string dirPath in Directory.GetDirectories(FullPath(sourcePath), "*", SearchOption.AllDirectories))
      Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

    foreach (string newPath in Directory.GetFiles(FullPath(sourcePath), "*.*",SearchOption.AllDirectories))
      File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
  }

  public static void CopyFile(string sourcePath, string destPath) 
   => File.Copy(FullPath(sourcePath), FullPath(destPath));

  public static object[] GetContent(string path) 
   => [..Directory.GetFiles(FullPath(path)), ..Directory.GetDirectories(FullPath(path))];

  public static string GetParent(string path) => FullPath(Directory.GetParent(path).FullName);

  public static void CreateDirectory(string dir) => Directory.CreateDirectory(FullPath(dir));

  public static bool IsFile(string path) => File.Exists(FullPath(path));

  public static bool Exist(string path) => Path.Exists(FullPath(path));
}