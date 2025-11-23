using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using Godot;
using monoe.exe.YumSharp;
using monoe.exe.YumSharp.Types;

namespace monoe.exe.Source.Core.Engine;

public partial class MonoeExeEngine : Node
{
  private static Assembly[] GetAssembliesFromDir(string dir)
  {
    List<Assembly> assemblies = [];
    if (Directory.Exists(dir))
    {
      var files = Directory.GetFiles(dir, "*.dll");
      foreach (var file in files) assemblies.Add(Execution.LoadExternalAssembly(file));
    }
    return [.. assemblies];
  }

  public override void _EnterTree()
  {
    GD.Print($"monoe.exe -- Based on YumEngine.{YumEngine.RuntimeInfo.WellVersionString()}");
    YumEngine.Init();
    
    string projPath = "./";

    foreach (var arg in OS.GetCmdlineArgs())
      if (Directory.Exists(arg)) projPath = arg;
    
    var runtimeFile = Path.Join(projPath, "monoe", "runtime.lua");
    var entryFile = Path.Join(projPath, "monomain.lua");
    myLuaState = new([typeof(void)]);
    int e;
    GD.Print($"Loading {runtimeFile}");
    if ((e = myLuaState.Load(runtimeFile, true)) != 0) Leave(e, $"Got exit {e} code when loading runtime file");
    GD.Print($"Loading {entryFile}");
    if ((e = myLuaState.Load(entryFile, true)) != 0) Leave(e, $"Got exit {e} code when loading runtime file");

    using YumVector exit = myLuaState.Call("main", [..OS.GetCmdlineArgs()]);
    if (exit.Count != 1) Leave(2, $"main returned {exit.Count} values... [{exit.Format(", ")}]");
    else if (exit[0].AsInt() != 0) Leave(2, $"main returned {exit[0].AsInt()}");
    
    using var libs = myLuaState.Call("_Mrtlibs", []);

    foreach (var libpath in libs)
    {
      if (libpath.IsString)
      {
        if (File.Exists(libpath))
          Assemblies.Add(Execution.LoadExternalAssembly(libpath));
        else if (Directory.Exists(libpath))
          Assemblies.AddRange(GetAssembliesFromDir(libpath));
        else throw new DllNotFoundException($"Cannot resolve {libpath.AsString()}");
      }
    }

    myLuaState.PushAssemblies([..GetEngineAssembly(), ..GetAssembliesFromDir(projPath)]);
  }

  public override void _Ready()
  {
    using var _ = myLuaState.Call("_Mrtready", []);
    
    YumEngine.IO.RedirectGOut((s) => {});
  }

  public override void _PhysicsProcess(double delta)
  {
    using YumVector _ = myLuaState.Call("_Mrtphysics_process", [delta]);
  }

  public override void _Process(double delta)
  {
    using YumVector _ = myLuaState.Call("_Mrtprocess", [delta]);
  }

  public override void _ExitTree()
  {
    myLuaState.Call("_Mrtexit", []);
    myLuaState.Dispose();
    YumEngine.Close();
  }

  private void Leave(int code = 0, string wah = "")
  {
    if (wah.Trim() != "") GD.PrintErr(wah);
    GetTree().Quit(code);
  }
}