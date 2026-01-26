using System;
using Godot;
using monoe.exe.Core.Bridge.Types.Interfaces;

namespace monoe.exe.Core.Bridge.Types.UI;

public class Container : Exposable, IScalable2D, IPositionable2D, IAttacher
{
  private readonly ScrollContainer container = new();
  private readonly Control innerContainer;

  public Container(string kind)
  {
    innerContainer = kind.ToLowerInvariant() switch
    {
      "vbox" => new VBoxContainer(),
      "hbox" => new HBoxContainer(),
      _ => throw new ArgumentException($"unknown container kind: {kind}"),
    };

    container.Size = new Vector2(400, 300);
    container.CustomMinimumSize = new Vector2(400, 300);

    innerContainer.CustomMinimumSize = new Vector2(400, 1000);

    container.AddChild(innerContainer);
  }

  public void Attach(object[] uids)
  {
    foreach (var uid in uids)
      if (uid is long luid) Expose(innerContainer, luid);
  }

  public object[] Deplace(double x, double y)
  {
    container.Position += new Vector2((float)x, (float)y);
    return [(double)container.Position.X, (double)container.Position.Y];
  }

  public object[] GetPosition()
  {
    return [(double)container.Position.X, (double)container.Position.Y];
  }

  public object[] GetScale()
  {
    return [(double)container.Scale.X, (double)container.Scale.Y];
  }

  public object[] GetSize()
  {
    return [(double)container.CustomMinimumSize.X, (double)container.CustomMinimumSize.Y];
  }

  public override Node NRef()
  {
    return container;
  }

  public override void Remove()
  {
    container.GetParent()?.RemoveChild(container);
  }

  public object[] Scale(double x, double y)
  {
    container.Scale += new Vector2((float)x, (float)y);
    return [(double)container.Scale.X, (double)container.Scale.Y];
  }

  public void SetPosition(double x, double y)
  {
    container.Position = new Vector2((float)x, (float)y);
  }

  public void SetScale(double x, double y)
  {
    container.Scale = new Vector2((float)x, (float)y);
  }

  public void SetSize(double x, double y)
  {
    container.Size = new Vector2((float)x, (float)y);
  }

  protected override void _Free()
  {
    container.QueueFree();
  }
}