using System;
using monoe.exe.YumSharp.Managed;

namespace monoe.exe.Core.Bridge;

public class Script : IDisposable
{
  private readonly string src;
  private readonly bool isFile = false;
  private YumState state = new();
  private readonly Action onerror = null;

  public object[] Call(string name, params object[] args)
  {
    try
    {
      return state.Call(name, args: args);
    } 
    catch (YumException e)
    {
      Utils.LuaErrorUtils.DumpLuaError(e, src);
      onerror?.Invoke();
    }

    return [];
  }
  public void PushCallback(string name, Func<object[], object[]> func) => state.PushCallback(name, func);
  public void PushCallback(string name, Action<object[]> func) => state.PushCallback(name, func);

  public void Dispose()
  {
    GC.SuppressFinalize(this);
    state.Dispose();
  }

  public void Reload()
  {
    state.Dispose();
    state = new();
    try
    {
      state.Run("function ready()end function process()end function physics()end function exit()end", false);
      state.Run(src, isFile);
    } 
    catch (YumException e)
    {
      Utils.LuaErrorUtils.DumpLuaError(e, src);
      onerror?.Invoke();
    }
  }

  public void Run(string s, bool isFile)
  {
    try
    {
      state.Run(s, isFile);
    } 
    catch (YumException e)
    {
      Utils.LuaErrorUtils.DumpLuaError(e, src);
      onerror?.Invoke();
    }
  }

  public Script(string source, bool isFile = true, Action onerror = null)
  {
    src = source;
    this.isFile = isFile;
    this.onerror = onerror;
    try
    {
      state.Run("function ready()end function process()end function physics()end function exit()end", false);
      state.Run(src, isFile);
    } 
    catch (YumException e)
    {
      Utils.LuaErrorUtils.DumpLuaError(e, src);
      onerror?.Invoke();
    }
  }

}