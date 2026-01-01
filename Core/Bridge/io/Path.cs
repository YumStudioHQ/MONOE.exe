using System.IO;

namespace monoe.exe.Core.Bridge.io;

public static class PathLib
{
  public static string FullPath(string of)
  {
    return Path.GetFullPath(of);
  }

  public static void CopyDirectory(string sourcePath, string targetPath)
  {
    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
      Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

    foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
      File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
  }

}