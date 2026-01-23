using System.Linq;
using Godot;
using monoe.exe.Core.Bridge.Types.Interfaces;
using monoe.exe.Core.Engine.Resources;

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

  public object[] Deplace(double x, double y)
  {
    label.Position += new Vector2((float)x, (float)y);
    return [label.Position.X, label.Position.Y];
  }

  public object[] GetPosition() => [label.Position.X, label.Position.Y];
  public object[] GetScale() => [label.Scale.X, label.Scale.Y];
  public object[] GetSize() => [label.Size.X, label.Size.Y];

  public object[] Scale(double x, double y)
  {
    label.Scale += new Vector2((float)x, (float)y);
    return [label.Scale.X, label.Scale.Y];
  }

  public void SetPosition(double x, double y) => label.Position = new((float)x, (float)y);
  public void SetScale(double x, double y) => label.Scale = new((float)x, (float)y);
  public void SetSize(double x, double y) => label.Size = new((float)x, (float)y);

  public void SetText(params object[] args) => label.Text = string.Join("", args.Select(obj => obj.ToString()));
  public string Text() => label.Text;

  public void SetFont(string path)
   => label.AddThemeFontOverride("font", new FontResource(path).GetFont());

  public void SetFontSize(long size)
   => label.AddThemeFontSizeOverride("font_size", (int)size);

  public void SetFontColor(double r, double g, double b, double a = 1.0)
  {
    label.AddThemeColorOverride(
      "font_color",
      new Color((float)r, (float)g, (float)b, (float)a)
    );
  }
}