using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace monoe.exe.Source.Core;

public static partial class Execution
{
  private static readonly AssemblyLoadContext GodotContext = AssemblyLoadContext.Default;

  public static Assembly LoadExternalAssembly(string path)
  {
    if (!File.Exists(path))
      throw new FileNotFoundException($"Assembly not found: {path}");

    AppDomain.CurrentDomain.AssemblyResolve += ResolveGodotAssembly;

    var assembly = GodotContext.LoadFromAssemblyPath(Path.GetFullPath(path));

    return assembly;
  }

  private static Assembly ResolveGodotAssembly(object sender, ResolveEventArgs args)
  {
    var requestedName = new AssemblyName(args.Name).Name;

    var godotAsm = AppDomain.CurrentDomain.GetAssemblies()
        .FirstOrDefault(a => string.Equals(a.GetName().Name, requestedName, StringComparison.OrdinalIgnoreCase));

    if (godotAsm != null) return godotAsm;

    return null;
  }
}