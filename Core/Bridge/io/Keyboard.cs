using Godot;
using System;
using System.Collections.Generic;

namespace monoe.exe.Core.Bridge.io;

public static class KeyResolver
{
  private static readonly Dictionary<string, Key> _map;

  static KeyResolver()
  {
    _map = new Dictionary<string, Key>();

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


public static class Keyboard
{
  public static bool KeyPressed(string key)
  {
    if (KeyResolver.TryResolve(key, out Key Gkey))
      return Input.IsPhysicalKeyPressed(Gkey);

    return false;
  }

  public static bool ActionPressed(string name) => Input.IsActionPressed(name);
  public static bool ActionJustPressed(string name) => Input.IsActionJustPressed(name);
  public static bool ActionReleased(string name) => Input.IsActionJustReleased(name);
}