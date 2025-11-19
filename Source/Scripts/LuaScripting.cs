using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using monoe.exe.YumSharp;
using monoe.exe.YumSharp.Natives;
using monoe.exe.YumSharp.Types;
using static monoe.exe.YumSharp.YumVectorExtensions;
using Script = monoe.exe.Source.Core.Script;

namespace monoe.exe.Source.Scripts;

[AttributeUsage(AttributeTargets.Method)]
public class LuaExportFunctionAttribute(string name, string ns = "Monoe") : Attribute
{
  public string Name { get; } = name;
  public string Namespace { get; } = ns;
}

public class LuaScripting : Script
{
  private readonly YumSubsystem subsystem = new();
  private readonly ulong yumSysUid;
  private readonly Dictionary<long, object> instances = [];
  private readonly Dictionary<string, Type> reflection = [];
  private long seed = Random.Shared.NextInt64();

  public override YumVector Call(string name, YumVector args)
   => subsystem.Call(yumSysUid, name, args);

  private static object PassAsInteger(YumVariant v, Type type)
  {
    return type switch
    {
      Type when type == typeof(byte) => (byte)v.AsInt(),
      Type when type == typeof(short) => (short)v.AsInt(),
      Type when type == typeof(ushort) => (ushort)v.AsInt(),
      Type when type == typeof(int) => (int)v.AsInt(),
      Type when type == typeof(uint) => (uint)v.AsInt(),
      Type when type == typeof(long) => v.AsInt(),
      Type when type == typeof(ulong) => (ulong)v.AsInt(),
      _ => throw new TargetInvocationException(new ArgumentException($"Variant holds {v.GetKind()} ; expected {type.FullName}"))
    };
  }

  private static object PassAsFloat(YumVariant v, Type type)
  {
    return type switch
    {
      Type when type == typeof(double) => v.AsFloat(),
      Type when type == typeof(float) => (float)v.AsFloat(),
      _ => throw new TargetInvocationException(new ArgumentException($"Variant holds {v.GetKind()} ; expected {type.FullName}"))
    };
  }

  private static bool PassAsBool(YumVariant v, Type type)
  {
    return type switch
    {
      Type when type == typeof(double) => v.AsBool(),
      _ => throw new TargetInvocationException(new ArgumentException($"Variant holds {v.GetKind()} ; expected {type.FullName}"))
    };
  }

  private static object PassAsString(YumVariant v, Type type)
  {
    return type switch
    {
      Type when type == typeof(string) => v.AsString(),
      Type when type == typeof(char[]) => v.AsString().ToCharArray(),
      Type when type == typeof(char) => v.AsString().FirstOrDefault(),
      Type when type == typeof(Memory<char>) => v.AsString().AsMemory(),
      _ => throw new TargetInvocationException(new ArgumentException($"Variant holds {v.GetKind()} ; expected {type.FullName}"))
    };
  }

  private static object PassAsBinary(YumVariant v, Type type)
  {
    return type switch
    {
      Type when type == typeof(byte[]) => v.AsBytes(),
      Type when type == typeof(Memory<byte>) => v.AsBytes().AsMemory(),
      _ => throw new TargetInvocationException(new ArgumentException($"Variant holds {v.GetKind()} ; expected {type.FullName}"))
    };
  }

  private static YumTable PassAsTable(YumVariant v, Type type)
  {
    return type switch
    {
      Type when type == typeof(YumTable) => v.AsTable(),
      _ => throw new TargetInvocationException(new ArgumentException($"Variant holds {v.GetKind()} ; expected {type.FullName}"))
    };
  }

  private object PassAsUID(YumVariant v, Type type)
  {
    if (type == typeof(YumUID)) return v.AsUID();
    if (instances.TryGetValue(v.AsUID().bytes, out object val))
      if (val.GetType() == type) return val;
    throw new TargetInvocationException(new ArgumentException($"Variant holds {v.GetKind()} ; expected {type.FullName}"));
  }

  private object StrictCastVariant(YumVariant v, Type parameterType)
   => v.GetKind() switch
   {
     YumKind.Integer => PassAsInteger(v, parameterType),
     YumKind.Float => PassAsFloat(v, parameterType),
     YumKind.Bool => PassAsBool(v, parameterType),
     YumKind.String => PassAsString(v, parameterType),
     YumKind.Binary => PassAsBinary(v, parameterType),
     YumKind.Table => PassAsTable(v, parameterType),
     YumKind.UID => PassAsUID(v, parameterType),
     _ => new object()
   };

  private long NextUID() => seed++;
  
  private long InlineWrap(object o)
  {
    var uid = NextUID();
    instances[uid] = o;
    return uid;
  }

  private string Dump(long uid, YumVector args, string callee)
   => $"object ID: {uid} | {instances.GetValueOrDefault(uid).GetType()?.FullName} | {args.Count} arguments (used {args.Count - 2})\n"
    + $"calling {callee}"
   ;

  private YumVector WrapeAndCall(object instance, MethodInfo method, YumVector variants)
  {
    var list = new List<object>();
    var @params = method.GetParameters();

    for (int i = 0; i < @params.Length; i++)
    {
      var p = @params[i];
      var v = variants[i];

      list.Add(StrictCastVariant(v, p.ParameterType));
    }

    var o = method?.Invoke(instance, [.. list]);

    return o switch
    {
      null => [],
      short s => [(long)s],
      ushort s => [(long)s],
      int i => [(long)i],
      uint i => [(long)i],
      ulong i => [(long)i],
      long i => [i],
      float f => [(double)f],
      double d => [d],
      string s => [s],
      char c => [new string(c, 1)],
      char[] ca => [new string(ca)],
      byte[] b => [b],
      byte b => [new string((char)b, 1)],
      YumVector v => v,
      YumVariant v => [v],
      YumTable t => [t],
      YumUID uid => [uid],
      _ => [InlineWrap(o), o.GetType().FullName],
    };
  }

  [LuaExportFunction("_typecall", "natives")]
  private YumVector CBL_typecall(YumVector args)
  {
    if (args.Count >= 2 && args[0].IsInt && args[1].IsString)
    {
      var uid = args[0].AsInt();
      var name = args[1].AsString();

      if (instances.TryGetValue(uid, out object o))
        try
        {
          MethodInfo method = o.GetType().GetMethod(name, (int)(args.Count-2), []);
          return WrapeAndCall(o, method, args.SliceFrom(2));
        }
        catch (Exception e)
        {
          GD.PrintErr($"Exception during a _typecall(...) invocation\t{e}\n{Dump(uid, args, name)}");
        }
    }
    return [];
  }

  [LuaExportFunction("_new", "natives")]
  private YumVector CBL_new(YumVector args)
  {
    if (args.Count >= 1 && args[0].IsString)
    {
      var typename = args[0].AsString();
      if (reflection.TryGetValue(typename, out Type type))
      {
        var key = NextUID();
        var ist = Activator.CreateInstance(type);
        instances[key] = ist;

        return [key];
      }
    }

    return [(long)-1];
  }

  [LuaExportFunction("_is_engine_init", "natives")]
  private static YumVector CBL_is_engine_init(YumVector _args) => [true];

  [LuaExportFunction("_get_methods_in_base", "natives")]
  private YumVector CBL_get_methods_in_base(YumVector args)
  {
    YumVector yums = [];
    foreach (var arg in args)
    {
      if (arg.IsInt && instances.TryGetValue(arg.AsInt(), out object val))
      {
        var methods = val.GetType()?.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                                    .Where(T => !yums.Any(y => y.AsString() == T.Name));
        foreach (var method in methods) yums.Add(method.Name);
      }
    }

    return yums;
  }

  [LuaExportFunction("_new_from", "natives")]
  private YumVector CBL_new_from(YumVector _args)
  {
    foreach (var arg in _args)
    {
      // return the first given valid uid argument
      if (arg.IsInt && instances.TryGetValue(arg.AsInt(), out object v))
      {
        var uid = NextUID();
        instances[uid] = Activator.CreateInstance(v.GetType());
        return [uid];
      }
    }
    return [];
  }

  [LuaExportFunction("_staticcall", "natives")]
  private YumVector CBL_staticcall(YumVector args)
  {
    // expects: [0]=string typename, [1]=string method, [2..n]=args
    if (args.Count < 2 || !args[0].IsString || !args[1].IsString)
      return [];

    var typename = args[0].AsString();
    var methodName = args[1].AsString();

    if (!reflection.TryGetValue(typename, out var type))
    {
      // try a runtime lookup for system types if not in reflection map
      type = Type.GetType(typename);
      if (type is null)
      {
        GD.PrintErr($"[Monola._staticcall] Type not found: {typename}");
        return [];
      }
    }

    try
    {
      var methodInfo = type.GetMethod(methodName,
          BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      if (methodInfo is null)
      {
        GD.PrintErr($"[Monola._staticcall] Static method not found: {typename}.{methodName}");
        return [];
      }

      return WrapeAndCall(null, methodInfo, args.SliceFrom(2));
    }
    catch (Exception e)
    {
      GD.PrintErr($"Exception during _staticcall(...) {typename}.{methodName}: {e}");
      return [];
    }
  }


  private LuaScripting()
  {
    yumSysUid = subsystem.NewState();
    if (!subsystem.Good(yumSysUid)) throw new InvalidOperationException("Got invalid UID (internal error)");

    var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance)
                 .Where(m => m.GetCustomAttribute<LuaExportFunctionAttribute>() != null);

    foreach (var method in methods)
    {
      var atr = method.GetCustomAttribute<LuaExportFunctionAttribute>();
      var parameters = method.GetParameters();
      bool correctSignature = parameters.Length == 1 &&
                              parameters[0].ParameterType == typeof(YumVector) &&
                              method.ReturnType == typeof(YumVector);

      if (!correctSignature)
      {
        GD.PushWarning("Trying to push unmatching function type..");
        continue;
      }

      Func<YumVector, YumVector> del;
      if (method.IsStatic)
      {
        del = (Func<YumVector, YumVector>)Delegate.CreateDelegate(typeof(Func<YumVector, YumVector>), method);
      }
      else
      {
        del = (Func<YumVector, YumVector>)Delegate.CreateDelegate(typeof(Func<YumVector, YumVector>), this, method);
      }

      subsystem.PushCallback(yumSysUid, atr?.Name ?? method?.Name ?? "_unnamed_function", del, atr?.Namespace ?? "Monoe");
    }
  }

  public LuaScripting(Dictionary<string, Type> types) : this()
  {
    reflection = types;
  }

  public LuaScripting(List<Type> types) : this()
  {
    Dictionary<string, Type> dict = [];
    foreach (var type in types) dict[type?.FullName ?? "undefined"] = type ?? typeof(object);
    reflection = dict;
  }

  public override int Load(string s, bool isFile = true)
   => subsystem.Load(yumSysUid, s, isFile);


  public override void Dispose()
  {
    subsystem.DeleteState(yumSysUid);
    subsystem.Dispose();
  }

  public override void PushAssemblies(Assembly[] assemblies)
  {
    foreach (var assembly in assemblies)
    {
      try
      {
        foreach (var type in assembly.GetTypes())
        {
          if (!reflection.ContainsKey(type.FullName))
          {
            reflection[type.FullName] = type;
          }
        }
      }
      catch (ReflectionTypeLoadException ex)
      {
        GD.PrintErr($"Error loading types from assembly {assembly.FullName}: {ex}");
      }
    }
  }
}