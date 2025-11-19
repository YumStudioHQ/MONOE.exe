using System;
using System.Reflection;
using monoe.exe.YumSharp.Types;

namespace monoe.exe.Source.Core;

public class Script : IDisposable
{
  public virtual YumVector Call(string name, YumVector args) => [];
  public virtual bool HasMethod(string name) => false;
  public virtual void Dispose() {}
  public virtual int Load(string s, bool isFile = true) => -1;
  public virtual void PushAssemblies(Assembly[] assemblies) { }
}