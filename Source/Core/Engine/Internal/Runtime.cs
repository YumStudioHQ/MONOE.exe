using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Godot;

namespace monoe.exe.Source.Core.Engine.Internal;

public partial class EngineClass
{
  public partial class Runtime(EngineClass _Aengine)
  {
    private readonly EngineClass _engine = _Aengine;

    private readonly List<Assembly> _assemblies = [];

    public bool Verbose { get; private set; } = false;

    public Assembly[] GetAssemblies()
    {
      return [.. GetEngineAssembly(), .. _assemblies,];
    }

    public string ExecutionPath { get; private set; } = "./";

    public void AddAssemblyDirectory(string dir)
    {
      if (Directory.Exists(dir))
      {
        var files = Directory.GetFiles(dir, "*.dll");
        foreach (var file in files)
          try
          {
            _assemblies.Add(_engine.LoadExternalAssembly(file));
          }
          catch (Exception e)
          {
            GD.PrintErr($"Got exception when loading file {file}\n{e}");
          }
      }
    }

    public void BootRuntime(string dir)
    {
      AddAssemblyDirectory(dir);
      AddAssemblyDirectory("./libs");
      AddAssemblyDirectory("./libraries");
      ExecutionPath = dir;
      _engine.LuaState.PushAssemblies(GetAssemblies());
    }
  }
}