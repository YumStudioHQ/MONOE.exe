using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge;

public static class Importer
{
  private const long invaliduid = -1;
  private static readonly List<Assembly> assemblies = [.. Engine.EngineAssembly.GetEngineAssembly()];
  private static readonly Dictionary<string, Type> types = [];
  private static readonly ConcurrentDictionary<(string type, string method), MethodInfo> staticMethodCache = new();

  public static Assembly[] GetAssemblies() => [.. assemblies];

  public static void LoadAssemblies(string[] strings)
  {
    assemblies.AddRange(Engine.EngineAssembly.GetAssemblies(strings));
    foreach (var asm in assemblies)
    {
      foreach (var type in asm.GetTypes())
      {
        types[type.FullName] = type;
      }
    }
  }

  public static void Clear()
  {
    assemblies.Clear();
    types.Clear();
    staticMethodCache.Clear();
  }

  public static object[] Limport(object[] args)
  {
    if (args.Length < 1 || args[0] is not string typeName)
      return [invaliduid, "bad arguments (expected type name as string)"];

    if (!types.TryGetValue(typeName, out var type))
      return [invaliduid, $"type '{typeName}' not found"];

    var ctorArgs = args.Skip(1).ToArray();

    var ctor = type.GetConstructors()
      .FirstOrDefault(c =>
      {
        var parameters = c.GetParameters();
        if (parameters.Length != ctorArgs.Length)
          return false;

        for (int i = 0; i < parameters.Length; i++)
        {
          if (ctorArgs[i] == null)
            continue;

          if (!parameters[i].ParameterType
                .IsAssignableFrom(ctorArgs[i].GetType()))
            return false;
        }

        return true;
      });

    if (ctor == null)
      return [invaliduid, $"no matching constructor found for '{type.FullName}'"];

    try
    {
      var instance = ctor.Invoke(ctorArgs);

      long uid = ObjectRegistry.Register(instance);

      if (instance is ManagedObject mo)
        mo.SetUID(uid);

      return [uid];
    }
    catch (Exception e)
    {
      return [invaliduid, e.InnerException?.ToString() ?? e.ToString()];
    }
  }

  public static object[] Lcall(object[] args)
  {
    if (args.Length >= 2 && args[0] is long uid && args[1] is string methodname)
    {
      if (ObjectRegistry.TryGet(uid, out object instance))
      {
        var method = instance.GetType()
            .GetMethod(methodname,
                 BindingFlags.Public
               | BindingFlags.Instance
               | BindingFlags.Static
               | BindingFlags.NonPublic)
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

  private static MethodInfo ResolveStaticMethod(string typeName, string methodName)
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
      staticMethodCache[key] = method ?? throw new MissingMethodException(
            $"static method {methodName} not found in {typeName}"
        );
      return method;
    }

    throw new TypeLoadException($"static base '{typeName}' not found");
  }

  public static object[] Lstaticcall(object[] args)
  {
    if (args.Length < 2 ||
        args[0] is not string @base ||
        args[1] is not string methodname)
      return [];

    var callArgs = args.Skip(2).ToArray();

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
}