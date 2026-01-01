namespace monoe.exe.Core.Settings;

public class GameSettings
{
  public bool HasShell { get; set; } = false;
  public bool HasHotReload { get; set; } = false;
  public string MainFile { get; set; } = "res/src/main.lua";
  public bool IsVerbose { get; set; } = false;
}