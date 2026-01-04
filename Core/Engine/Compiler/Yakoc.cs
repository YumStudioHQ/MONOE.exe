using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace monoe.exe.Core.Engine.Compiler;

public class Monoec
{
  public static void Compile()
  {
    Building.PrepareBuild();

    string code = """
                  namespace monoe.lib.Generated.Runtime 
                  { 
                    public class MonolibMainApp : monoe.exe.Core.Base.ReleaseBase
                    {
                      public Godot.Node Expose()
                      {
                        return new monoe.exe.Core.Base.MainBase(base.gameSettings);
                      }
                    }
                  }
    
                  """;
    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

    Assembly[] assemblies = [.. EngineAssembly.GetEngineAssembly(), 
                                typeof(Main).Assembly, 
                                typeof(Godot.Aabb).Assembly, 
                                typeof(Path).Assembly];
    var references = assemblies
        .Where(asm =>
        {
          var exists = File.Exists(asm.Location);
          if (!exists) EngineConsole.WriteLine($"{asm.Location}: Assembly not found", ConsoleColor.Yellow);
          return exists;
        })
        .Select(asm =>
          {
            EngineConsole.WriteLine($"> Using Assembly: {asm.Location}", ConsoleColor.DarkGray);

            if (!asm.FullName.StartsWith("System") && !asm.FullName.StartsWith("Microsoft"))
            {
              string buildPath = Path.Join(Building.GetAssembliesDir(), Path.GetFileName(asm.Location));
              File.Copy(asm.Location, buildPath, true);
            }

            return MetadataReference.CreateFromFile(asm.Location);
          })
        .ToArray();

    var compilation = CSharpCompilation.Create(
        "monoelib",
        [syntaxTree],
        references,
        new CSharpCompilationOptions(
          OutputKind.DynamicallyLinkedLibrary,
          allowUnsafe: true,
          nullableContextOptions: NullableContextOptions.Enable
        )
    );

    Directory.CreateDirectory(Path.GetDirectoryName(Building.GameAssemblyOutPath)!);

    using var fs = new FileStream(
        Building.GameAssemblyOutPath,
        FileMode.Create,
        FileAccess.Write,
        FileShare.None
    );

    var result = compilation.Emit(fs);

    if (!result.Success)
    {
      foreach (var diag in result.Diagnostics)
        EngineConsole.WriteError(diag.ToString());

      throw new InvalidOperationException("Roslyn compilation failed.");
    }

    fs.Flush();
    EngineConsole.Verbose(
        $"Assembly written: {Building.GameAssemblyOutPath} ({fs.Length} bytes)"
    );

    fs.Close();

    Building.BuildReleases();
  }
}
