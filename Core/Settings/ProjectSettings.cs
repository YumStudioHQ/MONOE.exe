using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Godot;
using monoe.exe.Core.Base;
using monoe.exe.Core.Engine;
using monoe.exe.Core.Engine.Shell;
using monoe.exe.YumSharp.Managed;

namespace monoe.exe.Core.Settings;

[AttributeUsage(AttributeTargets.Field)]
public sealed class SettingDescriptionAttribute : Attribute
{
  public string Text { get; }

  public SettingDescriptionAttribute(string text)
  {
    Text = text;
  }
}

public static class CurrentProject
{
  public static class PROJECT
  {
    [SettingDescription("Path to the project icon")]
    public static string ICON = "icon.png";

    [SettingDescription("Main developer name")]
    public static string DEV_NAME = "unknown";

    [SettingDescription("The company's name")]
    public static string COMPANY_NAME = "unknown";

    [SettingDescription("Enable debug features")]
    public static bool DEBUG = false;
  }

  public static class WINDOW
  {
    [SettingDescription("Initial window size, X ; Y")]
    public static Vector2I SIZE = new(1200, 720);

    [SettingDescription("Allow window resizing")]
    public static bool RESIZABLE = true;

    [SettingDescription("Window's title")]
    public static string TITLE = $"{PROJECT.COMPANY_NAME} project";

    [SettingDescription("If true, the Window's background can be transparent. This is best used with embedded windows.\nNote: Transparency support is implemented on Linux, macOS and Windows, but availability might vary depending on GPU driver, display manager, and compositor capabilities.")]
    public static bool TRANSPARENT = false;

    [SettingDescription("If non-zero, the Window can't be resized to be bigger than this size.")]
    public static Vector2I MAX_SIZE = new(0, 0);

    [SettingDescription("If non-zero, the Window can't be resized to be smaller than this size.")]
    public static Vector2I MIN_SIZE = new(0, 0);

    [SettingDescription("If true, the Window will be in exclusive mode. Exclusive windows are always on top of their parent and will block all input going to the parent Window.")]
    public static bool EXCLUSIVE = false;

    [SettingDescription("If true, the Window contents is expanded to the full size of the window, window title bar is transparent.")]
    public static bool EXTEND_TO_TITLE = false;
  }

  public static class ENGINE
  {
    [SettingDescription("If non-zero, limits the game's frame rate")]
    public static int MAX_FPS = 0;
   
    [SettingDescription("The speed multiplier at which the in-game clock updates, compared to real time. For example, if set to 2.0 the game runs twice as fast, and if set to 0.5 the game runs half as fast.")]
    public static int TIME_SCALE = 1;
  }
}

[ShellCommandHolder]
public static class MonoeProjectSettings
{
  private static readonly string luaCode =
@"local function get()
  for scopename, scope in pairs(_G) do
    if type(scope) == 'table' then
      for key, value in pairs(scope) do
        if type(value) ~= 'function'
        and type(value) ~= 'thread'
        and type(value) ~= 'userdata'
        and type(value) ~= 'table' then
          register(scopename, key, value)
        elseif type(value) == 'table' then
          register(scopename, key, table.unpack(value))
        end
      end
    end
  end
end

get()
";

  public static void LoadProject()
  {
    using YumState state = new(true);
    state.PushCallback("register", Lregister);

    var file = Application.ProjectSettingsFile();

    try
    {
      if (File.Exists(file))
      state.Run(file, true);
    }
    catch (Exception e)
    {
      EngineConsole.WriteError(e);
      MainBase.RequestExit();
    }

    state.Run(luaCode, false);
  }

  private static string ToScreamCase(string lua)
    => lua.ToUpperInvariant();

  private static string ToSnakeCase(string cs)
    => cs.ToLowerInvariant();

  private static object[] Lregister(object[] args)
  {
    if (args.Length < 3)
      return [];

    if (args[0] is not string scope ||
        args[1] is not string key)
      return [];

    var scopeName = scope.ToUpperInvariant();
    var fieldName = ToScreamCase(key);

    var root = typeof(CurrentProject);

    var scopeType = root.GetNestedType(
      scopeName,
      BindingFlags.Public | BindingFlags.Static
    );

    if (scopeType == null)
      return [];

    var field = scopeType.GetField(
      fieldName,
      BindingFlags.Public | BindingFlags.Static
    );

    if (field == null)
      return [];

    try
    {
      object value;

      if (args.Length == 3)
      {
        value = Convert.ChangeType(
          args[2],
          field.FieldType,
          CultureInfo.InvariantCulture
        );
      }
      else
      {
        var values = args.Skip(2).ToArray();

        if (field.FieldType == typeof(Vector2I) && values.Length == 2)
        {
          var x = Convert.ToInt32(values[0], CultureInfo.InvariantCulture);
          var y = Convert.ToInt32(values[1], CultureInfo.InvariantCulture);
          value = new Vector2I(x, y);
        }
        else if (field.FieldType == typeof(Vector2) && values.Length == 2)
        {
          var x = Convert.ToSingle(values[0], CultureInfo.InvariantCulture);
          var y = Convert.ToSingle(values[1], CultureInfo.InvariantCulture);
          value = new Vector2(x, y);
        }
        else
        {
          value = ConvertToArray(values, field.FieldType);
        }
      }

      field.SetValue(null, value);
    }
    catch
    {
      EngineConsole.WriteWarning(
        $"invalid value for settings property: {fieldName}"
      );
    }

    return [];
  }

  private static object ConvertToArray(object[] values, Type targetType)
  {
    // If field is object[], always allow
    if (targetType == typeof(object[]))
      return values;

    if (!targetType.IsArray)
      throw new InvalidOperationException("Target is not an array");

    var elemType = targetType.GetElementType()!;

    // Handle null-only arrays → object[]
    if (values.All(v => v is null))
    {
      if (targetType == typeof(object[]))
        return values;

      throw new InvalidOperationException("Null-only array not allowed here");
    }

    var array = Array.CreateInstance(elemType, values.Length);

    for (int i = 0; i < values.Length; i++)
    {
      var v = values[i];

      if (v == null)
      {
        array.SetValue(null, i);
        continue;
      }

      // Only allowed primitive types
      if (v is not long &&
          v is not double &&
          v is not bool &&
          v is not string)
      {
        throw new InvalidOperationException("Unsupported array element type");
      }

      array.SetValue(
        Convert.ChangeType(v, elemType, CultureInfo.InvariantCulture),
        i
      );
    }

    return array;
  }

  [ShellCommand("dump-settings", "dumps the project's settings")]
  public static void DumpSettings(string[] _)
  {
    EngineConsole.Verbose("dumping project settings ...");
    using var writer = new StreamWriter(Application.ProjectSettingsFile());
    writer.WriteLine($"-- monoe.exe engine's settings\n-- auto-gen glue, based on monoe.exe@{Version.All}\n");

    var root = typeof(CurrentProject);

    foreach (var scope in root.GetNestedTypes(
      BindingFlags.Public | BindingFlags.Static))
    {
      writer.WriteLine($"{ToSnakeCase(scope.Name)} = {{");

      foreach (var field in scope.GetFields(
        BindingFlags.Public | BindingFlags.Static))
      {
        var value = field.GetValue(null);

        var desc = field.GetCustomAttribute<SettingDescriptionAttribute>();
        var name = ToSnakeCase(field.Name);
        writer.Write("  ");
        writer.Write(name);
        writer.Write(" = ");
        writer.Write(LuaValue(value));

        if (desc != null)
        {
          writer.Write(", -- ");
          writer.Write(desc.Text.Replace('\n', ' '));
        }

        writer.WriteLine(",");
      }

      writer.WriteLine("}");
      writer.WriteLine();
    }

    EngineConsole.Verbose("dumped settings!", ConsoleColor.Green);
  }

  private static string LuaValue(object value)
  {
    if (value == null)
      return "nil";

    var type = value.GetType();

    if (value is Vector2 v2)
      return $"{{ {v2.X.ToString(CultureInfo.InvariantCulture)}, {v2.Y.ToString(CultureInfo.InvariantCulture)} }}";

    if (value is Vector2I v2i)
      return $"{{ {v2i.X.ToString(CultureInfo.InvariantCulture)}, {v2i.Y.ToString(CultureInfo.InvariantCulture)} }}";

    if (type.IsArray)
    {
      var array = (Array)value;

      if (array.Length == 0)
        return "{}";

      var sb = new System.Text.StringBuilder();
      sb.Append("{ ");

      for (int i = 0; i < array.Length; i++)
      {
        var elem = array.GetValue(i);
        sb.Append(LuaValue(elem));

        if (i < array.Length - 1)
          sb.Append(", ");
      }

      sb.Append(" }");
      return sb.ToString();
    }

    return value switch
    {
      string s => $"'{s.Replace("\\", "\\\\").Replace("'", "\\'")}'",
      bool b => b ? "true" : "false",
      long l => l.ToString(CultureInfo.InvariantCulture),
      int i => i.ToString(CultureInfo.InvariantCulture),
      double d => d.ToString(CultureInfo.InvariantCulture),
      float f => f.ToString(CultureInfo.InvariantCulture),
      _ => Convert.ToString(value, CultureInfo.InvariantCulture)
    };
  }

}
