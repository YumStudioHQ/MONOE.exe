using System;
using System.IO;
using System.Threading;
using monoe.exe.Core.Base;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Engine.Shell;
using monoe.exe.Core.Engine.Shell.Lua;
using monoe.exe.Core.Manager;
using monoe.exe.YumSharp.Managed;

namespace monoe.exe.Core.Bridge.Types.LibSys;

public class ManagedThread : ManagedObject
{
  private readonly Thread thread;
  private volatile bool shouldExit = false;
  private readonly object[] execParams;

  public ManagedThread(string code, string entry, string finished, bool libs)
  {
    execParams =
    [
      code,
      entry,
      finished,
      libs,
    ];

    thread = new Thread(Execute);
  }

  public void Start(params object[] args)
  {
    object[] threadArgs = [.. execParams, .. args];
    thread.Start(threadArgs);
  }

  private static void SetUp(YumState state)
  {
    state.PushCallback("monoe.import", Importer.Limport);
    state.PushCallback("monoe.call", Importer.Lcall);
    state.PushCallback("monoe.staticcall", Importer.Lstaticcall);
    state.PushCallback("monoe.wait",MainBase.Lsleep);
    state.PushCallback("monoe.shell", Shell.Prompt);
    string injection = """
                       monoe = monoe or {}
                       monoe.event = monoe.event or {}
                       monoe.event.emit = monoe.event.emit or function(name)end

                       print = function(...)
                         local args = {}
                         local t = { ... }
                         for _, value in pairs(t) do
                          args[#args + 1] = tostring(value)
                         end
                         monoe.staticcall("monoe.exe.Core.Engine.EngineConsole", "Print", table.unpack(args))
                       end
                       """;
    state.Run(injection, false);
  }

  private void Execute(object parameter)
  {
    if (parameter is not object[] parameters || parameters.Length < 4)
      throw new ArgumentException("Invalid parameters passed to thread execution.");

    string code = parameters[0] as string ?? throw new ArgumentException("Code must be a string.");
    string entry = parameters[1] as string ?? throw new ArgumentException("Entry must be a string.");
    string finished = parameters[2] as string ?? throw new ArgumentException("Finished must be a string.");
    bool libs = parameters[3] is bool b && b;

    object[] args = parameters.Length > 4 ? parameters[4..] : [];

    using YumState state = new(libs);
    state.Run(code, LuaCLIService.IsLuaFile(code));
    SetUp(state);
    state.PushCallback("monoe.exit_requested", (_) => { return [ShouldExit()]; });
    var result = state.Call(entry, args);

    if (ShouldExit()) MainBase.EnqueueOnMain(() =>
    {
      MainBase.Emit(finished, "exited");
    });
    else MainBase.EnqueueOnMain(() =>
    {
      MainBase.Emit(finished, result);
    });
  }

  public bool ShouldExit() => shouldExit;
  public void RequestExit() => shouldExit = true;

  public bool Terminate()
  {
    RequestExit();
    return thread.Join(500);
  }

  protected override void _Free()
  {
    if (thread.IsAlive)
    {
      EngineConsole.Verbose($"thread#{thread.Name} is not finished yet, waiting ...");
      RequestExit();
      if (thread.Join(2000))
      {
        EngineConsole.Verbose($"thread#{thread.Name} finished successfully");
      }
      else
      {
        EngineConsole.Verbose($"thread#{thread.Name} did not finish");
      }
    }
  }
}