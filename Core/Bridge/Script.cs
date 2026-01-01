using System;
using System.IO;
using monoe.exe.Core.Engine;
using monoe.exe.YumSharp.Managed;

namespace monoe.exe.Core.Bridge;

public class Script : YumState, IDisposable
{
  private readonly string src;
  private readonly bool isFile = false;
  private readonly Action onerror = null;

  public new object[] Call(string name, params object[] args)
  {
    try
    {
      return base.Call(name, args: args);
    } 
    catch (YumException e)
    {
      Utils.LuaErrorUtils.DumpLuaError(e, src);
      onerror?.Invoke();
    }

    return [];
  }

  public void Reload()
  {
    Clear();

    try
    {
      base.Run("function ready()end function process()end function physics()end function exit()end", false);
      base.Run(src, isFile);
    } 
    catch (YumException e)
    {
      Utils.LuaErrorUtils.DumpLuaError(e, src);
      onerror?.Invoke();
    }
  }

  public new void Run(string s, bool isFile = false)
  {
    try
    {
      base.Run(s, isFile);
    } 
    catch (YumException e)
    {
      Utils.LuaErrorUtils.DumpLuaError(e, src);
      onerror?.Invoke();
    }
  }

  public void RawRun(string s, bool isFile = false) => base.Run(s, isFile);

  public Script(string source, bool isFile = true, Action onerror = null)
  {
    src = source;
    this.isFile = isFile;
    this.onerror = onerror;
    base.Run($"package.path = package.path .. {EngineResources.LuaLibrariesFmt()}", false);

    if (isFile && !File.Exists(src)) throw new FileNotFoundException($"no such file {src}!");
    base.Run(src, isFile);
  }
}