using System.IO;

namespace monoe.exe.Core.Bridge.io;

public static class PathLib
{
  public static string FullPath(string of) => Path.GetFullPath(of);
  
  public static void CopyDirectory(string sourcePath, string targetPath)
  {
    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
      Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

    foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
      File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
  }

  public static void CopyFile(string sourcePath, string destPath) => File.Copy(sourcePath, destPath);
  public static object[] GetContent(string path) => [..Directory.GetFiles(path), ..Directory.GetDirectories(path)];
  public static string GetParent(string path) => FullPath(Directory.GetParent(path).FullName);
  public static void CreateDirectory(string dir) => Directory.CreateDirectory(dir);
  public static bool IsFile(string path) => File.Exists(path);
  public static bool Exist(string path) => Path.Exists(path);
}