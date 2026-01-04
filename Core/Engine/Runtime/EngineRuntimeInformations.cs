namespace monoe.exe.Core.Engine.Runtime;

public static class EngineRuntimeInformations
{
#if MONOE_RELEASE
  public static readonly bool IsRelease = true;
#else
  public static readonly bool IsRelease = false;
#endif
}