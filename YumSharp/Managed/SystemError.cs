using monoe.exe.YumSharp.Natives;

namespace monoe.exe.YumSharp.Managed;

public class SystemError
{
  private syserr_t err = new();

  public SystemError() {}

  public SystemError(syserr_t e)
  {
    err = e;
  }

  public string Source()
    => $"{Conversion.LStringToString(err.source.file)}:{err.source.line}:{Conversion.LStringToString(err.source.func)}";
  
  public string File() => Conversion.LStringToString(err.source.file);
  public string Func() => Conversion.LStringToString(err.source.func);
  public long Line() => err.source.line;

  public string Comment() => Conversion.LStringToString(err.comment);
  public syserr_category Category() => err.category;
  public static string StringCategory() => $"err.category";

  public override string ToString()
  {
    return Conversion.LStringToString(
      INative.yumfmterr(err)
    );
  }
}
