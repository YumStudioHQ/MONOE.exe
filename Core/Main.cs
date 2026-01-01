using System.IO;
using System.Linq;
using Godot;

namespace monoe.exe.Core;

public partial class Main : Base.MainBase
{
  public Main()
  {
    var query = OS.GetCmdlineArgs().Where(arg => !arg.StartsWith('-'))
                                   .Where(arg => File.Exists(arg))
                                   .ToArray();
    gameSettings = new()
    {
      HasHotReload = !OS.GetCmdlineArgs().Contains("-no-hot-reload"),
      HasShell = !OS.GetCmdlineArgs().Contains("-no-shell"),
      IsVerbose = !OS.GetCmdlineArgs().Contains("-silent"),
      MainFile = query.Length > 0 ? query[0] : (File.Exists("res/src/main.lua") ? "res/src/main.lua" : "main.lua")
    };
  }
}
