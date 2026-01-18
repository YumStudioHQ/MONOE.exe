using System;

namespace monoe.exe.Core.Engine.Shell;

[AttributeUsage(AttributeTargets.Method)]
public class ShellCommandAttribute(string name, string help = "", string[] args = null!) : Attribute
{
  public readonly string Help = help;
  public readonly string Name = name;
  public readonly string[] Arguments = args ?? [];
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ShellCommandDelegateAttribute : Attribute { }
