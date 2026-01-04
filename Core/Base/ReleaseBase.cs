using monoe.exe.Core.Engine.Resources.Release;
using monoe.exe.Core.Settings;

namespace monoe.exe.Core.Base;

public class ReleaseBase
{
  protected readonly GameSettings gameSettings = new()
  {
    MainFile = ReleaseAppResources.GetResourceDirectory("res", "main.lua"),
  };
}