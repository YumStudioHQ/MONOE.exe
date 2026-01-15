using System.Linq;
using Godot;
using monoe.exe.Core.Bridge.Types.Interfaces;

namespace monoe.exe.Core.Bridge.Types.UI;

public class TextLabel : Exposable, IScalable2D, IPositionable2D
{
  private readonly Label label = new()
  {
    Text = ""
  };

  public override Node NRef() => label;
  public override void Remove() => label.GetParent().RemoveChild(label);
  protected override void _Free() => label.QueueFree();

  object[] IPositionable2D.Deplace(double x, double y)
  {
    label.Position += new Vector2((float)x, (float)y);
    return [label.Position.X, label.Position.Y];
  }

  object[] IPositionable2D.GetPosition() => [label.Position.X, label.Position.Y];
  object[] IScalable2D.GetScale() => [label.Scale.X, label.Scale.Y];
  object[] IScalable2D.GetSize() => [label.Size.X, label.Size.Y];

  object[] IScalable2D.Scale(double x, double y)
  {
    label.Scale += new Vector2((float)x, (float)y);
    return [label.Scale.X, label.Scale.Y];
  }

  void IPositionable2D.SetPosition(double x, double y) => label.Position = new((float)x, (float)y);
  void IScalable2D.SetScale(double x, double y) => label.Scale = new((float)x, (float)y);
  void IScalable2D.SetSize(double x, double y) => label.Size = new((float)x, (float)y);
  
  public void SetText(params object[] args) => label.Text = string.Join("", args.Select(obj => obj.ToString()));
  public string Text() => label.Text;
}