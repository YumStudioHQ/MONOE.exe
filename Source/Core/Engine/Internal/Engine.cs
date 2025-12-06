using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using monoe.exe.YumSharp;

namespace monoe.exe.Source.Core.Engine.Internal;

public partial class EngineClass
{
  public static Assembly[] GetEngineAssembly()
  {
    return [.. AppDomain.CurrentDomain.GetAssemblies(),];
  }

  private readonly AssemblyLoadContext GodotContext = AssemblyLoadContext.Default;
  public Runtime RuntimeInstance { get; private set; }
  public LuaState LuaState { get; private set; }

  public EngineClass()
  {
    YumEngine.Init();
    RuntimeInstance = new(this);
    LuaState = new([typeof(void)]);
  }

  public Assembly LoadExternalAssembly(string path)
  {
    if (!File.Exists(path))
      throw new FileNotFoundException($"Assembly not found: {path}");

    AppDomain.CurrentDomain.AssemblyResolve += ResolveGodotAssembly;

    var assembly = GodotContext.LoadFromAssemblyPath(Path.GetFullPath(path));

    return assembly;
  }

  private Assembly ResolveGodotAssembly(object sender, ResolveEventArgs args)
  {
    var requestedName = new AssemblyName(args.Name).Name;

    var godotAsm = AppDomain.CurrentDomain.GetAssemblies()
        .FirstOrDefault(a => string.Equals(a.GetName().Name, requestedName, StringComparison.OrdinalIgnoreCase));

    if (godotAsm != null) return godotAsm;

    return null;
  }
}