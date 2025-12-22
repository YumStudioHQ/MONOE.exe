using Godot;

namespace monoe.exe.Core.Export;

public partial class RenderingServer : Node
{
  public RenderingServer()
  {
    GetTree().CurrentScene.AddChild(this);
  }

  
}