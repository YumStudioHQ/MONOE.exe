using System.IO;

namespace monoe.exe.Core.Bridge.io;

public static class PathLib
{
  public static string FullPath(string of)
  {
    return Path.GetFullPath(of);
  }
}