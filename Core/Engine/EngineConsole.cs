using System;
using System.Linq;
using System.Threading;

namespace monoe.exe.Core.Engine;

public static class EngineConsole
{
  // Lock object for thread safety
  private static readonly Lock consoleLock = new();
  public static bool IsVerbose = false;

  /// <summary>
  /// Thread-safe write with optional color
  /// </summary>
  public static void Write(string message, ConsoleColor? color = null)
  {
    lock (consoleLock)
    {
      var originalColor = Console.ForegroundColor;
      try
      {
        if (color.HasValue)
          Console.ForegroundColor = color.Value;

        Console.Write(message);
      }
      catch (Exception ex)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Console Write Error: " + ex.Message);
      }
      finally
      {
        Console.ForegroundColor = originalColor;
      }
    }
  }

  /// <summary>
  /// Thread-safe write line with optional color
  /// </summary>
  public static void WriteLine(string message, ConsoleColor? color = null)
  {
    lock (consoleLock)
    {
      var originalColor = Console.ForegroundColor;
      try
      {
        if (color.HasValue)
          Console.ForegroundColor = color.Value;

        Console.WriteLine(message);
      }
      catch (Exception ex)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Console WriteLine Error: " + ex.Message);
      }
      finally
      {
        Console.ForegroundColor = originalColor;
      }
    }
  }

  public static void WriteLine() => WriteLine("", null);
  public static void WriteLine(params object[] args) 
   => WriteLine(string.Join("", args.Select(arg => arg?.ToString() ?? "")));

  public static void Write(params object[] args)
   => Write(string.Join("", args.Select(arg => arg?.ToString() ?? "")));

  public static void Print(params object[] args)
  {
    WriteLine($"> {string.Join("\t", args.Select(arg => arg?.ToString() ?? ""))}");
  }

  public static void Verbose(params object[] args)
  {
    if (IsVerbose)
    {
      var time = DateTime.Now.ToString("HH:mm:ss.ffff");
      WriteLine($"[V] [{time}] {string.Join("\t", args.Select(arg => arg?.ToString() ?? ""))}", ConsoleColor.DarkGray);
    }
  }

  /// <summary>
  /// Thread-safe read line
  /// </summary>
  public static string ReadLine(string prompt = "> ", ConsoleColor? color = null)
  {
    var originalColor = Console.ForegroundColor;
    try
    {
      lock (consoleLock)
      {
        if (color != null) Console.ForegroundColor = color.Value;
        Console.Write(prompt);
        Console.ForegroundColor = originalColor;
      }
      return Console.ReadLine() ?? "";
    }
    catch (Exception ex)
    {
      WriteLine("[!] Console ReadLine Error: " + ex.Message, ConsoleColor.Red);
      return "";
    }
  }

  /// <summary>
  /// Thread-safe read a single key
  /// </summary>
  public static ConsoleKeyInfo ReadKey(string prompt = "")
  {
    lock (consoleLock)
    {
      try
      {
        if (!string.IsNullOrEmpty(prompt))
          Console.Write(prompt);

        return Console.ReadKey(true);
      }
      catch (Exception ex)
      {
        WriteLine("[!] Console ReadKey Error: " + ex.Message, ConsoleColor.Red);
        return new ConsoleKeyInfo();
      }
    }
  }

  /// <summary>
  /// Thread-safe write an error in red
  /// </summary>
  public static void WriteError(string message)
  {
    var time = DateTime.Now.ToString("HH:mm:ss.ffff");
    WriteLine($"[!] [{time}] {message}", ConsoleColor.Red);
  }

  public static void WriteError(Exception e)
  {
    WriteError($"{e}");
  }

  public static void WriteWarning(params object[] args)
  {
    var time = DateTime.Now.ToString("HH:mm:ss.ffff");
    WriteLine($"[w] [{time}] {string.Join("", args.Select(obj => obj.ToString() ?? ""))}", ConsoleColor.Yellow);
  }

  public static void WriteError(params object[] args)
  {
    var time = DateTime.Now.ToString("HH:mm:ss.ffff");
    WriteError(string.Join("", args.Select(obj => obj.ToString() ?? "")));
  }
}
