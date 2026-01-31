using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using monoe.exe.Core.Engine.Shell;

namespace monoe.exe.Core.Engine.Compiler.MonoeSharp;

[ShellCommandHolder]
public sealed class CSharpCompiler
{
  public sealed record CompilationResult(
    bool Success,
    IEnumerable<Diagnostic> Diagnostics
  );

  public static CompilationResult Compile(
      IEnumerable<string> sourceFiles,
      IEnumerable<string> referenceAssemblies,
      string outputPath,
      bool emitExe = false,
      string assemblyName = null,
      bool allowUnsafe = false,
      bool checkOverflow = true)
  {
    if (!sourceFiles.Any())
      throw new ArgumentException("No source files provided");

    assemblyName ??= Path.GetFileNameWithoutExtension(outputPath);

    var syntaxTrees = sourceFiles.Select(file =>
      CSharpSyntaxTree.ParseText(
        File.ReadAllText(file),
        path: file)
    );

    var references = new List<MetadataReference>();

    foreach (var refPath in referenceAssemblies)
    {
      var fullPath = Path.GetFullPath(refPath);

      if (!File.Exists(fullPath))
      {
        EngineConsole.WriteError($"reference not found: {fullPath}");
        continue;
      }

      references.Add(MetadataReference.CreateFromFile(fullPath));
    }

    var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

    foreach (var dll in Directory.GetFiles(runtimeDir, "*.dll"))
    {
      try
      {
        references.Add(MetadataReference.CreateFromFile(dll));
      }
      catch (BadImageFormatException)
      {
        // Native DLL → ignore
      }
    }

    references = [.. references
                  .GroupBy(r => r.Display)
                  .Select(g => g.First())
    ];

    var compilation = CSharpCompilation.Create(
      assemblyName,
      syntaxTrees,
      references,
      new CSharpCompilationOptions(
        outputKind: emitExe ? OutputKind.ConsoleApplication : OutputKind.DynamicallyLinkedLibrary,
        optimizationLevel: OptimizationLevel.Release,
        allowUnsafe: allowUnsafe, checkOverflow: checkOverflow
      )
    );

    using var outputStream = File.Create(outputPath);
    var emitResult = compilation.Emit(outputStream);

    return new CompilationResult(
      emitResult.Success,
      emitResult.Diagnostics
    );
  }

  [ShellCommand("cs", "Compiles C# code and generates an executable or a dynamic linked library.", ["[files...]", "[-unsafe: allows unsafe]", "[-check-overflow: checks overflows]", "[-dll|-shared: creates a Dynamic Linked Library]", "[-r [filename]: references the given assembly]", "[-o [filename]: indicates the output name]"])]
  public static void Compile(string[] args)
  {
    List<string> files = [];
    List<string> references = [];
    bool @unsafe = false;
    bool overflow = false;
    bool isDllOut = false;
    string output = "a.out";

    for (int i = 0; i < args.Length; i++)
    {
      var arg = args[i].Trim();
      if (arg == "-unsafe") @unsafe = true;
      else if (arg == "-check-overflow") overflow = true;
      else if (arg == "-dll" || arg == "-shared") isDllOut = true;
      else if (arg == "-o")
      {
        if (i + 1 >= args.Length)
        {
          EngineConsole.WriteError("expected output name after '-o'");
          return;
        }
        output = args[++i];
      }
      else if (arg == "-r")
      {
        if (++i >= args.Length)
        {
          EngineConsole.WriteError("expected assembly path after '-r'");
          return;
        }
        references.Add(args[i]);
      }
      else files.Add(args[i]);
    }

    var result = Compile([.. files], [.. references], output, !isDllOut, null, @unsafe, overflow);

    foreach (var diag in result.Diagnostics)
    {
      var msg = diag.ToString();
      if (diag.Severity == DiagnosticSeverity.Error)
        EngineConsole.WriteError(msg);
      else
        EngineConsole.WriteLine(msg);
    }
  }
}