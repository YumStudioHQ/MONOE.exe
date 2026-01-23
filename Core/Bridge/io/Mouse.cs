using Godot;

namespace monoe.exe.Core.Bridge.io;

public static class Mouse
{
  public static object[] Position()
  {
    var p = SceneRoot.I.GetViewport().GetMousePosition();
    return [(double)p.X, (double)p.Y];
  }

  public static object[] Delta()
  {
    var d = Input.GetLastMouseVelocity();
    return [(double)d.X, (double)d.Y];
  }

  public static bool ButtonPressed(string button)
  {
    return button.ToLowerInvariant() switch
    {
      "left"   => Input.IsMouseButtonPressed(MouseButton.Left),
      "right"  => Input.IsMouseButtonPressed(MouseButton.Right),
      "middle" => Input.IsMouseButtonPressed(MouseButton.Middle),
      "x1"     => Input.IsMouseButtonPressed(MouseButton.Xbutton1),
      "x2"     => Input.IsMouseButtonPressed(MouseButton.Xbutton2),
      _ => false
    };
  }
}
