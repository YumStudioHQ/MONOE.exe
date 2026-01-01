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
    PreBuild.PrepareBuild();

    string code = "namespace monoe.lib.Generated.Runtime; class MonolibMainApp : monoe.exe.Core.Base.MainBase {  }";
    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

    Assembly[] assemblies = [..EngineAssembly.GetEngineAssembly(), typeof(Main).Assembly, typeof(Godot.Aabb).Assembly, typeof(YumSharp.Managed.YumState).Assembly];
    var references = assemblies
        .Select(asm => asm.Location)
        .Where(path => !string.IsNullOrEmpty(path))
        .Select(path => 
          { 
            EngineConsole.WriteLine($"> Using Assembly: {path}", System.ConsoleColor.DarkGray);

            string buildPath = Path.Combine("build", Path.GetFileName(path));
            File.Copy(path, buildPath, true);
            EngineConsole.WriteLine($"> Copied: {buildPath}", System.ConsoleColor.DarkGray);

            return MetadataReference.CreateFromFile(path); 
          })
        .ToArray();

    var compilation = CSharpCompilation.Create(
        "monoelib",
        [syntaxTree],
        references,
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)
    );

    using var fs = new FileStream("build/monoe.lib.dll", FileMode.Create);
    var result = compilation.Emit(fs);

    if (!result.Success)
    {
      foreach (var diag in result.Diagnostics)
      {
        EngineConsole.WriteError(diag.ToString());
      }
    }
  }
}

/*
var assembly = Assembly.LoadFrom("GeneratedAssembly.dll");
var type = assembly.GetType("Test");
var method = type.GetMethod("Run");
method.Invoke(null, null);

*/