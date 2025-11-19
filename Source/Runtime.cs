using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Godot;
using monoe.exe.Source.Core;
using monoe.exe.Source.Scripts;
using monoe.exe.YumSharp;

namespace monoe.exe.Source;

public partial class Runtime : Node
{
  public static List<Type> GetEngineAssembly(Assembly[] assemblies)
  {
    Assembly[] full = [.. assemblies, .. AppDomain.CurrentDomain.GetAssemblies()];
    return [.. full
      .SelectMany(a =>
      {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
      })];
  }

  public override void _Ready()
  {
    GD.Print($"monoe.exe -- based on YumEngine.{YumEngine.RuntimeInfo.WellVersionString()}");
    YumEngine.Init();

    if (File.Exists("monoe/_main.lua")) // Boot on the main file
    {
      RTCore core = new()
      {
        Script = new LuaScripting([typeof(void), ..RuntimeAssemblyLoader.GetTypes(LoadLibraries())])
      };
      core.Load("monoe/_main.lua", true);
      AddChild(core);
    }
    else GD.PrintErr("monoe.exe: err: no boot file provided ! (monoe/_main.lua entry not found)");
  }

  private static Assembly[] GetAssembliesFromDir(string dir)
  {
    List<Assembly> assemblies = [];
    if (Directory.Exists(dir))
    {
      var files = Directory.GetFiles(dir, "*.dll");
      foreach (var file in files) assemblies.Add(RuntimeAssemblyLoader.LoadExternalAssembly(file));
    }
    return [.. assemblies];
  }

  private Assembly[] LoadLibraries()
  {
    GD.Print("monoe.exe: loading libraries...");
    List<Assembly> assemblies = [.. GetAssembliesFromDir("monoe/libs")];

    long i = 0;
    var args = OS.GetCmdlineArgs();
    foreach (var arg in args)
    {
      i++;
      if (arg == "-a")
      {
        if (i >= args.Length) GD.PushWarning($"monoe.exe: warn: expected argument after -a command (-a <AssemblySearchPath>)");
        else
        {
          var dir = args[i];
          if (Directory.Exists(dir)) assemblies.AddRange(GetAssembliesFromDir(dir));
          else GD.PushWarning($"monoe.exe: warn: no such assembly search path '{dir}'");
        }
      }
    }

    return [.. assemblies];
  }
}