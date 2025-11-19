using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Godot;

namespace monoe.exe.Source.Core;

public static class RuntimeAssemblyLoader
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

  public static Type[] GetTypes(Assembly[] assemblies)
  {
    List<Type> reflection = [];
    foreach (var assembly in assemblies)
    {
      try
      {
        foreach (var type in assembly.GetTypes())
        {
          reflection.Add(type);
        }
      }
      catch (ReflectionTypeLoadException ex)
      {
        GD.PrintErr($"Error loading types from assembly {assembly.FullName}: {ex}");
      }
    }
    return [..reflection];
  }
}