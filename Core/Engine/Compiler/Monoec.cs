namespace monoe.exe.Core.Engine.Compiler;

public class Monoec
{
  public static void Compile()
  {
    Building.PrepareBuild();
    Building.BuildReleases();
  }
}