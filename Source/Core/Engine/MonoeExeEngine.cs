using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace monoe.exe.Source.Core.Engine;

public partial class MonoeExeEngine : Node
{
  private readonly List<Assembly> Assemblies = [];
  private LuaState myLuaState;

  public Assembly[] GetEngineAssembly()
  {
    return [ ..AppDomain.CurrentDomain.GetAssemblies(), ..Assemblies];
  }
}