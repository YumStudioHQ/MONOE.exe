namespace monoe.exe.Core.Bridge.Types.Interfaces;

public interface IScalable2D
{
  public void SetSize(double x, double y);
  public void SetScale(double x, double y);

  public object[] GetSize();
  public object[] GetScale();

  public object[] Scale(double x, double y);
}