using System;
using System.Text.RegularExpressions;
using Godot;
using monoe.exe.YumSharp.Managed;

namespace monoe.exe.Core.Bridge.Utils;

public static partial class LuaErrorUtils
{
  private const string RESET = "\u001b[0m";
  private const string RED = "\u001b[31m";
  private const string GREEN = "\u001b[32m";
  private const string YELLOW = "\u001b[33m";
  private const string BLUE = "\u001b[34m";
  private const string MAGENTA = "\u001b[35m";
  private const string CYAN = "\u001b[36m";
  private const string RED_BG = "\u001b[41m";

  /// <summary>
  /// Dump Lua error with colored source snippet and syntax highlighting
  /// </summary>
  public static void DumpLuaError(YumException ex, string unit)
  {
    if (ex == null) return;

    string msg = ex.Message;
    var pattern = @"(?<file>[\w./\\]+\.lua):(?<line>\d+): (?<message>.+?)(?:$|\sfrom)";
    var matches = Regex.Matches(msg, pattern);

    if (matches.Count > 0)
    {
      foreach (Match match in matches)
      {
        string file = match.Groups["file"].Value;
        int line = int.Parse(match.Groups["line"].Value);
        string message = match.Groups["message"].Value;

        GD.PrintErr($"{RED}---- LUA ERROR ----{RESET}");
        GD.PrintErr($"{YELLOW}File:{RESET} {file}");
        GD.PrintErr($"{YELLOW}Line:{RESET} {line}");
        GD.PrintErr($"{YELLOW}Message:{RESET} {RED}{message}{RESET}");

        try
        {
          var lines = System.IO.File.ReadAllLines(file);
          int start = Math.Max(line - 4, 0);
          int end = Math.Min(line + 2, lines.Length - 1);

          GD.Print($"{CYAN}Source snippet:{RESET}");
          for (int i = start; i <= end; i++)
          {
            string code = lines[i];

            // Syntax highlight
            code = HighlightLuaSyntax(code);

            string prefix = (i + 1 == line) ? $"{RED_BG}>> {i + 1}: {code}{RESET} -- {message}" : $"   {i + 1}: {code}";
            GD.Print(prefix);
          }

          GD.Print($"{MAGENTA}From unit:{RESET} {unit}");
          GD.Print($"{RED}-------------------{RESET}");
        }
        catch
        {
          GD.Print($"{RED}Could not read Lua source file.{RESET}");
        }
      }
    }
    else
    {
      GD.PrintErr($"{RED}Lua error (could not parse file/line):{RESET} {msg}");
    }
  }

  private static string HighlightLuaSyntax(string line)
  {
    // Keywords
    string[] keywords = ["function", "end", "if", "then", "else", "elseif", "for", "in", "do", "while", "repeat", "until", "return", "local"];
    foreach (var kw in keywords)
    {
      line = Regex.Replace(line, $@"\b{kw}\b", $"{BLUE}{kw}{RESET}");
    }

    line = MyRegex().Replace(line, match => $"{GREEN}{match.Value}{RESET}"); // Yeah that's unredeable ash...
    line = MyRegex1().Replace(line, match => $"{YELLOW}{match.Value}{RESET}");
    line = MyRegex2().Replace(line, match => $"{CYAN}{match.Value}{RESET}");

    return line;
  }

  [GeneratedRegex("\".*?\"|'.*?'")]
  private static partial Regex MyRegex();
  [GeneratedRegex(@"\b\d+(\.\d+)?\b")]
  private static partial Regex MyRegex1();
  [GeneratedRegex("--.*$")]
  private static partial Regex MyRegex2();
}
