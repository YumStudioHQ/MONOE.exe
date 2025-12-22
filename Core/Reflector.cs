using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace monoe.exe.Core;

public class Reflector(string[] files)
{
  public static List<Type> GetTypes(Assembly[] assemblies)
  {
    Assembly[] full = [.. assemblies];
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

  private Assembly[] assemblies = [.. GetAssemblies(files), .. GetEngineAssembly()];
  private readonly ConcurrentDictionary<long, object> instances = [];
  private readonly ConcurrentDictionary<(string type, string method), MethodInfo> staticMethodCache = new();
  private long uidpos = 1;
  private const long invaliduid = -1;

  public object[] Limport(object[] args)
  {
    string err = "";
    if (args.Length >= 1 && args[0] is string @base)
    {
      foreach (var asm in assemblies)
      {
        var selections = asm.GetTypes().Where(t => t.FullName == @base).ToArray();
        if (selections.Length != 0)
        {
          var selection = selections[0];
          long uid = uidpos++;

          var ctor = selection.GetConstructor(Type.EmptyTypes);
          if (ctor == null)
            return [invaliduid, $"type {selection.FullName} has no parameterless constructor"];

          object instance = ctor.Invoke(null);
          instances[uid] = instance;
          return [uid];
        }
      }
      err = $"base '{@base}' not found";
    }
    else
    {
      err = $"bad arguments (expected string at position #1, got {args[0].GetType().FullName})";
    }

    return [invaliduid, err];
  }

  public object[] Lcall(object[] args)
  {
    if (args.Length >= 2 && args[0] is long uid && args[1] is string methodname)
    {
      if (instances.TryGetValue(uid, out object instance))
      {
        var method = instance.GetType()
            .GetMethod(methodname,
                 BindingFlags.Public
               | BindingFlags.Instance
               | BindingFlags.Static)
               ?? throw new MissingMethodException($"method {methodname} not found in base {instance.GetType().FullName}");

        var callArgs = args.Skip(2).ToArray();

        object target = method.IsStatic ? null : instance;
        object result;
        if (method.GetParameters() is [{ ParameterType: var t }]
            && t == typeof(object[]))
        {
          result = method.Invoke(target, [callArgs]);
          return result is object[] arrl ? arrl : [result];
        }

        result = method.Invoke(target, callArgs);
        return result is object[] arr ? arr : [result];
      }

      throw new Exception($"{uid}: No such internal UID.");
    }

    return [];
  }

  private MethodInfo ResolveStaticMethod(string typeName, string methodName)
  {
    var key = (typeName, methodName);

    if (staticMethodCache.TryGetValue(key, out var cached))
      return cached;

    foreach (var asm in assemblies)
    {
      var type = asm.GetType(typeName, throwOnError: false);
      if (type == null)
        continue;

      // ensure it's a static class (optional but good)
      if (!(type.IsAbstract && type.IsSealed))
        throw new Exception($"{typeName} is not a static class");

      var method = type.GetMethod(
          methodName,
          BindingFlags.Public | BindingFlags.Static
      );

      if (method == null)
        throw new MissingMethodException(
            $"static method {methodName} not found in {typeName}"
        );

      staticMethodCache[key] = method;
      return method;
    }

    throw new TypeLoadException($"static base '{typeName}' not found");
  }

  public object[] Lstaticcall(object[] args)
  {
    if (args.Length < 2 ||
        args[0] is not string @base ||
        args[1] is not string methodname)
      return [];

    var callArgs = args.Skip(2).ToArray();

    try
    {
      var method = ResolveStaticMethod(@base, methodname);

      object result;

      if (method.GetParameters() is [{ ParameterType: var t }] &&
          t == typeof(object[]))
      {
        result = method.Invoke(null, [callArgs]);
        return result is object[] arr ? arr : [result];
      }

      result = method.Invoke(null, callArgs);
      return result is object[] arr2 ? arr2 : [result];
    }
    catch (Exception e)
    {
      return [invaliduid, e.Message];
    }
  }
}