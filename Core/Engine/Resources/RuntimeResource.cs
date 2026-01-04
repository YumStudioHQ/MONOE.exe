namespace monoe.exe.Core.Engine.Resources;

public class RuntimeResource(string name, string path, string resources)
{
  public string Name { get; private set; } = name;

  /// <summary>
  /// Path to the runtime.
  /// </summary>
  public string Path { get; private set; } = path;

  /// <summary>
  /// Relative path to the resrouces folder.
  /// </summary>
  public string ResourcesRelative { get; private set; } = resources;
}