using System;
using System.Collections.Generic;
using monoe.exe.YumSharp.Natives;

namespace monoe.exe.YumSharp.Types;

public class YumSubsystem : IDisposable
{
  private IntPtr handle;
  private bool disposed;

  public YumSubsystem()
  {
    handle = INative.YumSubsystem_new();
    if (handle == IntPtr.Zero)
      throw new Exception("Failed to create subsystem");
  }

  public ulong NewState(bool loadStdLibs = true) =>
      INative.YumSubsystem_newState(handle, loadStdLibs ? 1 : 0);

  public void DeleteState(ulong uid) =>
      INative.YumSubsystem_deleteState(handle, uid);

  public bool IsValidUID(ulong uid) =>
      INative.YumSubsystem_isValidUID(handle, uid) != 0;

  public int Load(ulong uid, string source, bool isFile) =>
      INative.YumLuaSubsystem_load(handle, uid, source, isFile);

  public bool Good(ulong uid) =>
      INative.YumLuaSubsystem_good(handle, uid);

  public YumVector Call(ulong uid, string name, YumVector args)
  {
    var res = INative.YumLuaSubsystem_call(handle, uid, name, args.Handle);
    return new YumVector(res, true);
  }

  public void PushCallback(ulong uid, string name, Func<YumVector, YumVector> func, string ns = "")
  {
    INative.YumCallback cb = (inVecPtr, outVecPtr) =>
    {
      try
      {
        var input = new YumVector(inVecPtr, ownsHandle: false);

        // Borrowed output → does not own
        var outputVec = new YumVector(outVecPtr, ownsHandle: false);

        var result = func(input);

        foreach (var v in result) outputVec.Add(v);
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"[YumSubsystem] Callback '{name}' threw: {ex}");
        // IMPORTANT: swallow exception, don’t let it escape to Inative
      }
    };

    _callbacks.Add(cb); // keep alive

    var result = INative.YumLuaSubsystem_pushCallback(handle, uid, name, cb, ns);
    if (result != 0)
      throw new Exception($"Failed to push callback {name}");
  }

  public bool HasMethod(ulong uid, string path)
  {
    return INative.YumLuaSubsystem_hasMethod(handle, uid, path) != 0;
  }

  // Store delegates to prevent GC
  private readonly List<Delegate> _callbacks = [];

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this); // don’t run finalizer if already disposed
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposed)
    {
      if (handle != IntPtr.Zero)
      {
        INative.YumSubsystem_delete(handle);
        handle = IntPtr.Zero;
      }

      disposed = true;
    }
  }

  ~YumSubsystem()
  {
    Dispose(false);
  }
}