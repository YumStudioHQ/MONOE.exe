using Godot;

namespace monoe.exe.Source.Core;

public partial class RTCore : Node
{
  public Script Script { get; set; } = new();

  public int Load(string path, bool isFile = true) => Script.Load(path, isFile);

  public override void _Ready()
  {
    Script.Call("_init", []);
  }

  public override void _Process(double delta)
  {
    Script.Call("_process", [delta]);
  }

  public override void _ExitTree()
  {
    Script.Call("_exit", []);
    Script.Dispose();
  }
}