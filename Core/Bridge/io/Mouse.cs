namespace monoe.exe.Core.Bridge.io;

public static class Mouse
{
  public static object[] Position()
  {
    var pos = SceneRoot.I.GetViewport().GetMousePosition();
    return [pos.X, pos.Y];
  }
}