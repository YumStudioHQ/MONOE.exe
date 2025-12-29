using Godot;

namespace monoe.exe.Core.Bridge.Types;

public class QuickCamera2D : Exposable
{
  private readonly Camera2D cam = new();

  public override Node NRef()
   => cam;
  
}