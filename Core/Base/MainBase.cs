using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using Godot;
using monoe.exe.Core.Bridge;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Engine.Resources;
using monoe.exe.Core.Engine.Shell;
using monoe.exe.Core.Settings;
using monoe.exe.YumSharp.Managed;
using Script = monoe.exe.Core.Bridge.Script;
using Timer = Godot.Timer;

namespace monoe.exe.Core.Base;

public partial class MainBase : Control
{
  private static Script mainState = null;
  protected GameSettings gameSettings;
  private FileSystemWatcher watcher;
  private static readonly ConcurrentQueue<Action> mainThreadQueue = new();
  private static bool locked = false;
  private Action luaErrorHandler = null;
  private Timer cleanupTimer;
  public readonly static ConcurrentQueue<Action> GarbageCollector = [];
  private static Action reloadRequestAction = () => { };
  private static Action<int> exitRequestionAction = (_) => { };
  private volatile bool exitTreeCalled = false;

  public static void Emit(string @event, params object[] args)
  {
    mainState?.Call("monoe.event.emit", [@event, .. args]);
  }

  public static object[] LCall(string method, params object[] args)
  {
    return mainState.Call(method, args);
  }

  public static void EnqueueOnMain(Action action) => mainThreadQueue.Enqueue(action);

  public static void RequestReload()
  {
    EngineConsole.WriteLine();
    EngineConsole.Verbose("reload requested");
    mainThreadQueue.Enqueue(reloadRequestAction);
  }
  public static void RequestLock()
  {
    EngineConsole.Verbose((locked ? "un" : "") + "lock requested...");
    mainThreadQueue.Enqueue(() =>
    {
      EngineConsole.Verbose((locked ? "un" : "") + "locking...");
      /* Note:
       *  * This won't lock directly, event after executing the action, 
       *    as other action may be queued after this one !
       */
      locked = !locked;
    });
  }

  public static void RequestExit(long code = 0)
  {
    EnqueueOnMain(() =>
    {
      if (Application.IsShuttingDown)
      {
        EngineConsole.Verbose("exit rejected: already exiting!");
        return;
      }

      Application.IsShuttingDown = true;
      exitRequestionAction((int)code);
    });
  }

  public static void InvokeOnMainThreadBlocking(Action action)
  {
    using var done = new ManualResetEventSlim(false);
    Exception error = null;

    mainThreadQueue.Enqueue(() =>
    {
      try
      {
        action();
      }
      catch (Exception e)
      {
        error = e;
      }
      finally
      {
        done.Set();
      }
    });

    done.Wait();

    if (error != null) throw error;
  }

  public static void Run(string code) // This Run method is only for string injections!
   => mainState.Run(code, false);

  public MainBase()
  {
    EngineConsole.WriteLine($"monoe.exe meta-runtime | monoe.exe@{Version.All}");
  }

  public override void _EnterTree()
  {
    EngineConsole.IsVerbose = gameSettings.IsVerbose;
    EngineConsole.Verbose("monoe.exe: booting...");
    EngineConsole.Verbose($"engine resources path {EngineResources.GetResourceDir()}");

    /*
     * Before booting the engine, set up the ErrorHandler, and the FileSystemWatcher.
     */

    luaErrorHandler = () =>
    {
      locked = true;
    };

    if (/* Hot reload is optional! */ gameSettings.HasHotReload)
    {
      watcher = new(Directory.GetParent(gameSettings.MainFile).FullName)
      {
        NotifyFilter = NotifyFilters.Attributes
                     | NotifyFilters.CreationTime
                     | NotifyFilters.DirectoryName
                     | NotifyFilters.FileName
                     | NotifyFilters.LastAccess
                     | NotifyFilters.LastWrite
                     | NotifyFilters.Security
                     | NotifyFilters.Size
      };
      watcher.Changed += OnFileChanged;
      watcher.Filter = "*.lua";
      watcher.IncludeSubdirectories = true;
      watcher.EnableRaisingEvents = true;
    }

    /*
     * We also setup SceneRoot so we can use Godot through C# and Assembly without passing 
     * and keeping references to a node.
     */
    SceneRoot.SetNode(this);

    /*
     * Also expose a static action that allow other classes to request a reload!
     */
    reloadRequestAction = () =>
    {
      EngineConsole.Verbose("reloading...");
      Manager.ObjectRegistry.Clear();
      mainState.Reload();
      LoadProject();
    };

    // And exit!

    exitRequestionAction = (c) =>
    {
      GetTree().Quit(c);
    };
  }

  public override void _Ready()
  {
    EngineConsole.Verbose("loading project");

    /*
     * At the first frame, we call the `deps()` function first (in order to request needed libraries), and then,
     * once all library loaded, we call the `main()` function.
     */
    try
    {
      LoadProject();
    } catch (YumException e)
    {
      EngineConsole.WriteError(e);
    } catch (Exception e)
    {
      EngineConsole.WriteError("cannot load project!");
      EngineConsole.WriteError(e);
      Application.IsShuttingDown = true;
      GetTree().Quit(-1);
    }

    EngineConsole.Verbose("project ready!");
    EngineConsole.Verbose($"project path: {Path.GetFullPath(gameSettings.MainFile)}");
    EngineConsole.Verbose($"devs: {CurrentProject.PROJECT.DEV_NAME}, from {CurrentProject.PROJECT.COMPANY_NAME}");

    /*
     * Some events are fired at a fixed point, so people can "free a bit later"
     */
    SetUpTimers();

    if (gameSettings.HasDiagnostics) AddChild(new Engine.Layers.DebugLayer());

    if (gameSettings.HasShell)
    {
      bool canPass = false;
      foreach (var osArg in OS.GetCmdlineArgs())
      {
        var arg = osArg;

        if (arg.Length > 0 && canPass)
        {
          if (arg.StartsWith('-')) arg = ':' + arg[1..];
          Shell.ExecuteCommand(arg);
        }

        if (arg == "-c") canPass = !canPass;
      }

      var thread = new Thread(Shell.Prompt)
      {
        IsBackground = true,
      };

      thread.Start();
    }

    GetTree().AutoAcceptQuit = false;
  }

  public override void _Process(double delta)
  {
    if (Application.IsShuttingDown) return;
    /*
     * Seen as `process` event in Lua, this function is designed in order to update the game.
     * If you need to update physics, use the `physics` event instead.
     */
    while (!mainThreadQueue.IsEmpty)
    {
      if (mainThreadQueue.TryDequeue(out Action action)) action();
      else EngineConsole.WriteError(new Exception("Cannot dequeue `reloadQueue`"));
    }

    if (!locked)
    {
      Emit("@process", delta);
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    if (Application.IsShuttingDown) return;
    /*
     * Updates physics.
     */
    if (!locked)
    {
      Emit("@physics", delta);
      Emit("@collect");
    }
  }

  public override void _ExitTree()
  {
    if (exitTreeCalled)
    {
      return;
    }

    exitTreeCalled = true;

    EngineConsole.WriteLine();
    EngineConsole.Verbose("exit requested...");
    EngineConsole.Verbose("exit event fired");
    Emit("@onexit");
    Emit("@collect");
    Emit("@cleanup");
    Manager.ObjectRegistry.Clear();
    ResourceManager.Clear();
    watcher?.Dispose();

    while (!GarbageCollector.IsEmpty)
    {
      if (GarbageCollector.TryDequeue(out Action action))
      {
        action();
      }
      else EngineConsole.WriteError("[rejected]: Failled to deque an element !");
    }

    mainState?.Dispose();

    EngineConsole.Verbose("process finished");
  }

  public override void _Input(InputEvent @event)
  {
    Emit("@input");
  }

  public static object[] Lsleep(object[] args)
  {
    if (args.Length > 0)
    {
      if (args[0] is long l) Thread.Sleep((int)l);
      else if (args[0] is double d) Thread.Sleep((int)d);
    }
    return [];
  }

  public override void _Notification(int what)
  {
    if (what == NotificationWMCloseRequest)
    {
      mainState.Call("monoe.exit_requested", 0);
      return;
    }
  }

  private void LoadProject()
  {
    MonoeProjectSettings.LoadProject();
    SetUp.Launch(this);

    mainState = new(Path.GetFullPath(gameSettings.MainFile), true, luaErrorHandler);

    // After issue #29, as the editor itself ... does not have it (yet).
    mainState.Run("deps = deps or function()end", false);

    // 2. Load dependencies
    var libs = mainState.Call("deps")
                   .Where(o => o is string s && string.IsNullOrEmpty(s.Trim()))
                   .Cast<string>()
                   .ToArray();

    // 3. Load them.
    Importer.LoadAssemblies(libs);

    /* 4. Push callbacks.
     * Note: these callbacks are "visible" in monolib.lua and unique_event.lua files!
     * But you can absolutely use them without these files — They are designed only for IDEs!
     */
    mainState.PushCallback("monoe.import", Importer.Limport);
    mainState.PushCallback("monoe.call", Importer.Lcall);
    mainState.PushCallback("monoe.staticcall", Importer.Lstaticcall);
    mainState.PushCallback("monoe.wait", Lsleep);
    mainState.PushCallback("monoe.shell", Shell.Prompt);
    string injection = $$"""
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

                       {{LoadRuntimeInformations()}}
                       monoe.exit_requested = function(code) monoe.info.os.exit(code or 0) end
                       """;
    mainState.Run(injection, false);

    // 5. Call main.
    Run("main = main or function() end");

    var margs = mainState.Call("main");

    // 6. Load scripts (They are generally requested from the main function!)
    Emit("@load");

    // 7. Finally, call ready!
    Emit("@ready", margs);

    /* Quick note!
     * Generally, users love hot reloading. So, the function 'load' in monoe allows
     * hot reloading on other files than the main lua script. Idea is to subscribe a function
     * that'll load the file to two events: the @load event, and the @hot (that allows hot reloading).
     * When a hot reloading event is fired, each subscribers looks for the path: if it's their file, they'll 
     * reload the file.
     * That's also why the "ready" event is fired after loading: ready is for class (or whatever) initialization!
     *
     * Note: Your file will be in _G.module_name!
     */
  }

  private void SetUpTimers()
  {
    /* Quick note!
     * There is indeed two cleaning events ;
     * * @collect, called after each physic process,
     * * @cleanup, called at a fixed point (not something stable through versions!)
     * Depending what you're doing, you may change your event's usage!
     */
    cleanupTimer = new()
    {
      WaitTime = 1.5,
      OneShot = false,
    };

    cleanupTimer.Timeout += () => { Emit("@cleanup"); };

    AddChild(cleanupTimer);
  }

  private static string LoadRuntimeInformations()
  => $$"""
     monoe.info = {
       os = {
         name = '{{OS.GetName()}}',
         version = '{{OS.GetVersion()}}',
         argv = {{{GetFormatedCmdlineArgs()}}},
         processorcount = {{System.Environment.ProcessorCount}},
         isos64 = {{System.Environment.Is64BitOperatingSystem.ToString().ToLower()}},
         isproc64 = {{System.Environment.Is64BitProcess.ToString().ToLower()}},
         ispriviliged = {{System.Environment.IsPrivilegedProcess.ToString().ToLower()}},
         machinename = '{{System.Environment.MachineName.Replace("\'", "\\\'")}}',
         procid = {{System.Environment.ProcessId}},
         exit = function(code) monoe.staticcall('monoe.exe.Core.Base.MainBase', 'RequestExit', code or 0) end
       },
       runtime = {
         version = '{{Version.All}}',
         isdev = {{(Application.IsDevMode ? "true" : "false")}},
         iseditor = {{(Application.IsEditor ? "true" : "false")}},
       }
     }
     """;

  private static string GetFormatedCmdlineArgs()
  {
    string s = "{";

    foreach (var arg in OS.GetCmdlineArgs())
      s += $"'{arg}',";

    return s + '}';
  }

  private void OnFileChanged(object sender, FileSystemEventArgs e)
  {
    if (e.ChangeType != WatcherChangeTypes.Changed) return;
    EngineConsole.WriteLine($"\n> file changed {e.FullPath}", ConsoleColor.DarkGray);

    if (Path.GetFullPath(e.FullPath) == Path.GetFullPath(gameSettings.MainFile))
    {
      EngineConsole.Verbose("requested reboot...");
      mainThreadQueue.Enqueue(() =>
      {
        EngineConsole.Verbose("rebooting...");
        Manager.ObjectRegistry.Clear();
        LoadProject();
      });
    }
    else
    {
      mainThreadQueue.Enqueue(() =>
      {
        Emit("@hot", e.FullPath);
      });
    }

    if (locked)
    {
      locked = false;
    }
  }
}
