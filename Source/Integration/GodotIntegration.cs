using System;
using System.Collections.Generic;
using Godot;
using monoe.exe.Source.Core;
using monoe.exe.YumSharp.Types;
using Internal = monoe.exe.Source.Core.Engine.Internal;

namespace monoe.exe.Source.Integration;

public partial class GodotIntegration : Node
{
  private readonly Dictionary<long, Node> _pins;
  private long _pos = (long)Time.GetUnixTimeFromSystem();
  private Internal.EngineClass _engine;

  private long NextUID()
  {
    return _pos++;
  }

  private YumVector NewNode(YumVector args)
  {
    using var vec = new YumVector();

    return vec;
  }
}