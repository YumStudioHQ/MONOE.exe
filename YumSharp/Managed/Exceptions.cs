using System;
using monoe.exe.YumSharp.Natives;

namespace monoe.exe.YumSharp.Managed;

public class YumException : Exception
{
  public YumException() { }

  public YumException(string message) 
    : base(message) { }

  public YumException(string message, Exception innerException) 
    : base(message, innerException) { }

  public YumException(syserr_t err) 
    : base($"Engine error: {Conversion.LStringToString(INative.yumfmterr(err))}") {}
}
