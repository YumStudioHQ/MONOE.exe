using System;
using Godot;
using monoe.exe.YumSharp.Natives;

namespace monoe.exe.YumSharp
{
  public static class YumEngine
  {
    private static bool isInit = false;
    
    public static class RuntimeInfo
    {
      public static string Name() => INative.SafeIt(INative.YumEngineInfo_name());
      public static string StudioName() => INative.SafeIt(INative.YumEngineInfo_studioName());
      public static string StudioBranch() => INative.SafeIt(INative.YumEngineInfo_branch());
      public static int Major() => INative.YumEngineInfo_versionMajor();
      public static int Minor() => INative.YumEngineInfo_versionMinor();
      public static int Patch() => INative.YumEngineInfo_versionPatch();

      private static int CompareVersion(int aMaj, int aMin, int aPatch, int bMaj, int bMin, int bPatch)
      {
        if (aMaj != bMaj) return aMaj.CompareTo(bMaj);
        if (aMin != bMin) return aMin.CompareTo(bMin);
        return aPatch.CompareTo(bPatch);
      }

      public static bool RequireMin(int maj, int min, int patch)
      {
        var cmp = CompareVersion(Major(), Minor(), Patch(), maj, min, patch);
        return cmp >= 0;
      }

      public static bool RequireMin(Vector3I v) => RequireMin(v.X, v.Y, v.Z);

      public static bool RequireMax(int maj, int min, int patch)
      {
        var cmp = CompareVersion(Major(), Minor(), Patch(), maj, min, patch);
        return cmp <= 0;
      }

      public static bool RequireMax(Vector3I v) => RequireMax(v.X, v.Y, v.Z);

      public static bool IsSameVersion(Vector3I v)
      {
        return v.X == Major() && v.Y == Minor() && v.Z == Patch();
      }

      public static bool Require(Vector3I min, Vector3I max, Vector3I[] excludes)
      {
        if (!RequireMin(min)) return false;
        if (!RequireMax(max)) return false;

        if (excludes != null)
        {
          foreach (var exclude in excludes)
            if (IsSameVersion(exclude)) return false;
        }

        return true;
      }

      public static string VersionString()
      {
        return $"{Major()}.{Minor()}.{Patch()}";
      }

      public static string WellVersionString()
      {
        return $"{StudioName()}.{StudioBranch()}.{VersionString()}";
      }
    }

    public static void Close() => INative.YumCloseAPI();
    public static void EXPLODE() => INative.YUM_EXPLODE();

    public static void Init()
    {
      if (isInit) return;
      INative.YumEngine_init();
      AppDomain.CurrentDomain.ProcessExit += (_, _) => INative.YumCloseAPI();
      AppDomain.CurrentDomain.UnhandledException += (_, _) => INative.YumCloseAPI();
      Console.CancelKeyPress += (_, _) => INative.YumCloseAPI();
      isInit = true;
    }

    public static class IO
    {
      private static INative.YumRedirectionCallback _gOutCallback;
      private static INative.YumRedirectionCallback _gErrCallback;

      public static void RedirectGOut(Action<string> action)
      {
        _gOutCallback = msg =>
        {
          try
          {
            if (msg != null)
              action(msg);
          }
          catch (Exception ex)
          {
            GD.PrintErr($"[Yum IO] RedirectGOut error: {ex}");
          }
        };

        INative.Yum_redirect_G_out(_gOutCallback);
      }

      public static void RedirectGErr(Action<string> action)
      {
        _gErrCallback = msg =>
        {
          try
          {
            if (msg != null)
              action(msg);
          }
          catch (Exception ex)
          {
            GD.PrintErr($"[Yum IO] RedirectGOut error: {ex}");
          }
        };

        INative.Yum_redirect_G_err(_gOutCallback);
      }

      public static void OpenGOut(string path)
      {
        INative.Yum_open_G_out(path);
      }

      public static void OpenGErr(string path)
      {
        INative.Yum_open_G_err(path);
      }

      public static void OpenGIn(string path)
      {
        INative.Yum_open_G_in(path);
      }
    }
  }
}
