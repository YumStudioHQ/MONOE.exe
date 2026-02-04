using System.IO;
using System.Security.Cryptography;

namespace monoe.exe.Core.Engine.Resources.Cryptography;

public static class ResourceCrypting
{
  public static byte[] GetSHA256FromFile(string filepath)
  {
    var sha = SHA256.Create();
    using var fileStream = File.Open(filepath, FileMode.Open);
    return sha.ComputeHash(fileStream);
  }
}