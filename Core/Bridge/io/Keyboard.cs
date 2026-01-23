using Godot;
using System;
using System.Collections.Generic;

namespace monoe.exe.Core.Bridge.io;

public static class KeyResolver
{
  private static readonly Dictionary<string, Key> _map;

  static KeyResolver()
  {
    _map = [];

    foreach (Key k in Enum.GetValues<Key>())
    {
      string name = Normalize(k.ToString());
      _map[name] = k;
    }

    Alias("space", Key.Space);
    Alias("esc", Key.Escape);
    Alias("escape", Key.Escape);
    Alias("enter", Key.Enter);
    Alias("return", Key.Enter);
    Alias("left", Key.Left);
    Alias("right", Key.Right);
    Alias("up", Key.Up);
    Alias("down", Key.Down);
  }

  private static void Alias(string name, Key key)
      => _map[Normalize(name)] = key;

  private static string Normalize(string s)
      => s.Replace("_", "")
          .Replace("-", "")
          .Replace(" ", "")
          .ToLowerInvariant();

  public static bool TryResolve(string input, out Key key)
      => _map.TryGetValue(Normalize(input), out key);
}

/// <summary>
/// Primitive-only keyboard input bridge.
/// Exposes physical keys and Godot actions using strings and booleans.
/// </summary>
public static class Keyboard
{
  /// <summary>
  /// Returns true if a physical key is currently pressed.
  /// Example keys: "a", "space", "enter", "esc", "left", "right"
  /// </summary>
  public static bool KeyPressed(string key)
  {
    return KeyResolver.TryResolve(key, out var k)
      && Input.IsPhysicalKeyPressed(k);
  }

  /// <summary>
  /// Returns true if a Godot input action is currently pressed.
  /// </summary>
  public static bool ActionPressed(string action)
    => Input.IsActionPressed(action);

  /// <summary>
  /// Returns true if a Godot input action was just pressed this frame.
  /// </summary>
  public static bool ActionJustPressed(string action)
    => Input.IsActionJustPressed(action);

  /// <summary>
  /// Returns true if a Godot input action was just released this frame.
  /// </summary>
  public static bool ActionJustReleased(string action)
    => Input.IsActionJustReleased(action);

  /// <summary>
  /// Returns the strength of a Godot input action (0.0 → 1.0).
  /// Useful for analog input.
  /// </summary>
  public static double ActionStrength(string action)
    => Input.GetActionStrength(action);

  /// <summary> True if Shift is pressed </summary>
  public static bool Shift() => Input.IsKeyPressed(Key.Shift);

  /// <summary> True if Ctrl is pressed </summary>
  public static bool Ctrl() => Input.IsKeyPressed(Key.Ctrl);

  /// <summary> True if Alt is pressed </summary>
  public static bool Alt() => Input.IsKeyPressed(Key.Alt);

  /// <summary> True if Meta / Super is pressed </summary>
  public static bool Meta() => Input.IsKeyPressed(Key.Meta);
}