using System.IO;
using Godot;
using monoe.exe.Source.Core.Engine.Internal;
using monoe.exe.YumSharp;
using monoe.exe.YumSharp.Types;

namespace monoe.exe.Source.Core.Engine;

public partial class MonoeEngine : Node
{
  private EngineClass _engine = new();

  public override void _EnterTree()
  {
    GD.Print($"monoe.exe -- Based on YumEngine.{YumEngine.RuntimeInfo.WellVersionString()}");
    YumEngine.Init();
    _engine.RuntimeInstance.BootRuntime(Directory.GetCurrentDirectory());

    var runtimeFile = Path.Join("./", "monoe", "runtime.lua");
    var entryFile = Path.Join("./", "main.lua");
    int e;
    GD.Print($"Loading {runtimeFile}");
    if ((e = _engine.LuaState.Load(runtimeFile, true)) != 0) Leave(e, $"Got exit {e} code when loading runtime file");
    GD.Print($"Loading {entryFile}");
    if ((e = _engine.LuaState.Load(entryFile, true)) != 0) Leave(e, $"Got exit {e} code when loading runtime file");

    using var exit = _engine.LuaState.Call("main", [..OS.GetCmdlineArgs()]);
    if (exit.Count != 1) Leave(2, $"main returned {exit.Count} values... [{exit.Format(", ")}]");
    else if (exit[0].AsInt() != 0) Leave(2, $"main returned {exit[0].AsInt()}");
  }

  public override void _Ready()
  {
    using var _ = _engine.LuaState.Call("_ready", YumVector.UnsafeGlobalEmptyVector);
  }

  public override void _PhysicsProcess(double delta)
  {
    using YumVariant del = delta;
    using YumVector vec = [del];
    using var _ = _engine.LuaState.Call("_physics_process", vec);
  }

  public override void _Process(double delta)
  {
    using YumVariant del = delta;
    using YumVector vec = [del];
    using var _ = _engine.LuaState.Call("_process", vec);
  }

  public override void _ExitTree()
  {
    _engine.LuaState.Call("_exit", YumVector.UnsafeGlobalEmptyVector);
    _engine.LuaState.Dispose();
    YumEngine.Close();
  }

  private void Leave(int code = 0, string wah = "")
  {
    if (wah.Trim() != "") GD.PrintErr(wah);
    GetTree().Quit(code);
    YumEngine.Close();
  }

  public static void TESTAPP()
  {
    GD.Print("Yup");
  }
}