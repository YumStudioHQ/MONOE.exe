using Godot;

namespace monoe.exe.Core.Bridge;

public partial class SceneRoot : Node
{
  public static Node I;

  public static void SetNode(Node n) => I = n;
}
