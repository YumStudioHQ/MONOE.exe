using System.Collections.Generic;

namespace monoe.exe.Core.Engine.Shell;

public static class Parser
{
  public static List<string> SplitShellArgs(string input)
  {
    var args = new List<string>();
    var current = new System.Text.StringBuilder();

    char? quote = null;
    bool escaping = false;

    for (int i = 0; i < input.Length; i++)
    {
      char c = input[i];

      if (escaping)
      {
        current.Append(c);
        escaping = false;
        continue;
      }

      if (c == '\\')
      {
        escaping = true;
        continue;
      }

      if (quote != null)
      {
        if (c == quote) quote = null;
        else current.Append(c);

        continue;
      }

      if (c is '\'' or '"' or '`')
      {
        quote = c;
        continue;
      }

      if (char.IsWhiteSpace(c))
      {
        if (current.Length > 0)
        {
          args.Add(current.ToString());
          current.Clear();
        }
        continue;
      }

      current.Append(c);
    }

    if (escaping) current.Append('\\');

    if (current.Length > 0)
      args.Add(current.ToString());

    return args;
  }
}