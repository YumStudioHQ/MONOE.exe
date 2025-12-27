using System.Collections.Generic;
using Godot;
using monoe.exe.Core.Manager;

namespace monoe.exe.Core.Bridge.Types;

public class Animation2D : Exposable
{
  protected AnimatedSprite2D animation;

  public Animation2D()
  {
    animation = new()
    {
      SpriteFrames = new()
    };
  }

  public void NewAnimation(string name)
  {
    if (!animation.SpriteFrames.HasAnimation(name))
      animation.SpriteFrames.AddAnimation(name);
    animation.SpriteFrames.SetAnimationLoop(name, true);
  }

  private void _AddFrame(string name, Image frame, float duration, int pos)
   => animation.SpriteFrames.AddFrame(name, frame.Texture, duration, pos);

  public void AddFrame(string name, long uid, double duration, long pos)
  {
    if (ObjectRegistry.TryGet(uid, out Image frame))
    {
      _AddFrame(name, frame, (float)duration, (int)pos);
    }
    else throw new KeyNotFoundException($"UID: {uid} is not an image");
  }

  public void Play(string name)
   => animation.Play(name);

  public void PlayBackwards(string name)
   => animation.PlayBackwards(name);

  public void Pause() => animation.Pause();

  public object[] GetAnimations()
   => animation.SpriteFrames.GetAnimationNames();


  public void SetPosition(double x, double y)
  {
    animation.Position = new((float)x, (float)y);
  }

  public object[] GetPosition()
  {
    return [animation.Position.X, animation.Position.Y];
  }

  public object[] Move(double x, double y)
  {
    animation.Position = new(animation.Position.X + (float)x, animation.Position.Y + (float)y);
    return [animation.Position.X, animation.Position.Y];
  }

  public object[] Scale(double x, double y)
  {
    animation.Scale = new((float)x, (float)y);
    return [animation.Scale.X, animation.Scale.Y];
  }

  private void _AnimationFromImage(
  string name,
  Image image,
  int frameWidth,
  int frameHeight,
  int fromColumn,
  int toColumn,
  int fromRow,
  int toRow,
  double fps
)
  {
    int textureWidth = image.Texture.GetWidth();
    int textureHeight = image.Texture.GetHeight();

    int columns = textureWidth / frameWidth;
    int rows = textureHeight / frameHeight;

    // Clamp so we never go out of bounds
    fromColumn = Mathf.Clamp(fromColumn, 0, columns - 1);
    toColumn = Mathf.Clamp(toColumn, 0, columns - 1);
    fromRow = Mathf.Clamp(fromRow, 0, rows - 1);
    toRow = Mathf.Clamp(toRow, 0, rows - 1);

    if (!animation.SpriteFrames.HasAnimation(name))
      animation.SpriteFrames.AddAnimation(name);

    animation.SpriteFrames.SetAnimationLoop(name, true);
    animation.SpriteFrames.SetAnimationSpeed(name, fps);

    for (int y = fromRow; y <= toRow; y++)
    {
      for (int x = fromColumn; x <= toColumn; x++)
      {
        Rect2 region = new(
          x * frameWidth,
          y * frameHeight,
          frameWidth,
          frameHeight
        );

        AtlasTexture atlas = new()
        {
          Atlas = image.Texture,
          Region = region
        };

        animation.SpriteFrames.AddFrame(name, atlas);
      }
    }
  }

  public void AnimationFromImage(string name,
    long uid,
    long frameWidth,
    long frameHeight,
    long fromColumn,
    long toColumn,
    long fromRow,
    long toRow,
    double fps)
  {
    if (ObjectRegistry.TryGet(uid, out Image image))
    {
      _AnimationFromImage(name, image, (int)frameWidth, (int)frameHeight, (int)fromColumn, (int)toColumn, (int)fromRow, (int)toRow, fps);
    }
    else throw new KeyNotFoundException($"UID: {uid} is not an image");
  }

  public void FlipH(bool state)
  {
    animation.FlipH = state;
  }

  public void FlipV(bool state)
  {
    animation.FlipV = state;
  }

  protected override void _Free()
  {
    animation.QueueFree();
  }

  public override Node NRef()
  {
    return animation;
  }
}
