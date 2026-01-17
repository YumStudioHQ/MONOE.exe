namespace monoe.exe.Core.Engine;

public static class AppLifetime
{
  public static volatile bool IsShuttingDown = false;
  public static volatile bool IsReloading = false;
}
