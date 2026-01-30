namespace monoe.exe.Core.Settings;

public class GameSettings
{
  public bool HasShell { get; set; } = false;
  public bool HasHotReload { get; set; } = false;
  public string MainFile { get; set; } = "res/main.lua";
  public bool IsVerbose { get; set; } = false;
  public string[] LuaSearchDirectories { get; set; } = [];
  public bool HasDiagnostics { get; set; } = false;
}