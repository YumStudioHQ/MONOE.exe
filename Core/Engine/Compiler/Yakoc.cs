using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Linq;
using System.Reflection;

namespace monoe.exe.Core.Engine.Compiler;

public class Yakoc
{
  public static void Compile()
  {
    Building.PrepareBuild();

    string code = "namespace monoe.lib.Generated.Runtime; class MonolibMainApp : monoe.exe.Core.Base.MainBase {  }";
    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

    Assembly[] assemblies = [..EngineAssembly.GetEngineAssembly(), typeof(Main).Assembly, typeof(Godot.Aabb).Assembly, typeof(YumSharp.Managed.YumState).Assembly];
    var references = assemblies
        .Where(asm => !asm.GetName().Name.StartsWith("System") 
               && !asm.GetName().Name.StartsWith("Microsoft")
               && !asm.GetName().Name.StartsWith("netstandard"))
        .Select(asm => asm.Location)
        .Where(path => !string.IsNullOrEmpty(path))
        .Where(path =>
        {
          var exists = File.Exists(path);
          if (!exists) EngineConsole.WriteLine($"{path}: Assembly not found", System.ConsoleColor.Yellow);
          return exists;
        })
        .Select(path => 
          { 
            EngineConsole.WriteLine($"> Using Assembly: {path}", System.ConsoleColor.DarkGray);

            string buildPath = Path.Join(Building.GetBuildDir(), Path.GetFileName(path));
            File.Copy(path, buildPath, true);

            return MetadataReference.CreateFromFile(path); 
          })
        .ToArray();

    var compilation = CSharpCompilation.Create(
        "monoelib",
        [syntaxTree],
        references,
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)
    );

    using var fs = new FileStream(Path.Join(Building.GetBuildDir(), "monoe.lib.dll"), FileMode.Create);
    var result = compilation.Emit(fs);

    if (!result.Success)
    {
      foreach (var diag in result.Diagnostics)
      {
        EngineConsole.WriteError(diag.ToString());
      }
    }

    Building.BuildReleases();
  }
}
