using Godot;
using monoe.exe.Core.Base;

namespace monoe.exe.Core.Bridge.Types.LibSys;

public static class ManagedTimer
{
  public static void SetTimer(string eventName, double duration, bool oneShot)
  {
    Timer timer = new()
    {
      OneShot = oneShot,
      WaitTime = duration
    };

    timer.Timeout += () =>
    {
      MainBase.EnqueueOnMain(() => 
      {
        MainBase.Emit(eventName);
        if (oneShot) timer.QueueFree();
      });
    };

    SceneRoot.I.AddChild(timer);
    timer.Start();
  }
}