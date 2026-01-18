using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace monoe.exe.Core.Engine;

public static class EngineAssembly
{
  public static Type[] GetTypes(params Assembly[] assemblies)
  {
    Assembly[] full = [.. assemblies, ..GetEngineAssembly()];
    return [.. full
      .SelectMany(a =>
      {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
      })];
  }

  public static Assembly[] GetEngineAssembly()
   => [.. AppDomain.CurrentDomain.GetAssemblies()];

  public static Assembly[] GetAssemblies(string[] files)
  {
    List<Assembly> assemblies = [];

    foreach (var file in files)
    {
      if (string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file)) continue;
      Assembly assembly = Assembly.LoadFrom(file);
      assemblies.Add(assembly);
    }

    return [.. assemblies];
  }
}