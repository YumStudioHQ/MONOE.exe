namespace monoe.exe.Core.Bridge.Types.Interfaces;

public interface IPositionable2D
{
  public void SetPosition(double x, double y);
  public object[] GetPosition();
  public object[] Move(double x, double y);
}