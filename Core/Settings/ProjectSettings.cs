using System.Collections.Generic;
using System.Globalization;

namespace monoe.exe.Core.Settings;

public static class MonoeProjectSettings
{
  public class SettingsValue
  {
    public SettingsValue(object o) => _hold = o;
    public SettingsValue() { }

    private object _hold = null;

    private T GetT<T>(T @default)
    {
      if (_hold is T t) return t;
      return @default;
    }

    public long Integer() => GetT<long>(0);
    public string String() => GetT("");
    public bool Boolean() => GetT(false);
    public double Float() => GetT(0.0);

    public void Set(object value) => _hold = value;

    public static SettingsValue FromString(string str)
    {
      str = str.Trim();

      if (bool.TryParse(str, out bool b))
        return new SettingsValue(b);

      if (long.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out long l))
        return new SettingsValue(l);

      if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
        return new SettingsValue(d);

      if ((str.StartsWith('\"') && str.EndsWith('\"')) ||
          (str.StartsWith('\'') && str.EndsWith('\'')))
      {
        return new SettingsValue(str[1..^1]);
      }

      return new SettingsValue(str);
    }
  }

  private readonly static Dictionary<string, SettingsValue> settings = [];
  
}