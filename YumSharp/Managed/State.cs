using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using monoe.exe.YumSharp.Natives;

namespace monoe.exe.YumSharp.Managed;

public class YumState : IDisposable
{
  private nint _state = nint.Zero;
  private readonly List<object> _gpins = [];
  private bool disposed = false;

  private string Ensure(string path)
  {
    var parts = path.Split('.');
    var beg = $"{string.Join('.', parts[0..^1])}".TrimSuffix(".");
    var end = parts[^1];
    GD.Print($"beg:{beg}\tend:{end}");
    INative.libyum_ensure_path(_state, beg);
    return end;
  }

  public YumState(bool libs = true)
  {
    _state = INative.libyum_new();
    if (libs) INative.libyum_open_libs(_state);
  }

  public void Run(string src, bool isFile = false)
  {
    var ok = INative.libyum_run(_state, src, (sbyte)(isFile ? 1 : 0));
    if (ok.category != syserr_category.OK) throw new YumException(ok);
  }

  public unsafe object[] Call(string name, params object[] args)
  {
    var pinnedStrings = new List<byte[]>(args.Length);
    var variants = new variant_t[args.Length];
    var output = new List<object>();

    for (int i = 0; i < args.Length; i++)
    {
      variants[i] = Conversion.ObjectToVariant(args[i], pinnedStrings);
    }

    fixed (variant_t* pArgs = variants)
    {
      // Example native signature:
      // syserr_t libyum_call(
      //   nint state,
      //   lstring_t name,
      //   variant_t* args,
      //   ulong argc,
      //   out variant_t* rets, // Alocated by YumEngine!
      //   out ulong retc
      // );
      ulong outc;
      variant_t *outa = null;
      var err = INative.libyum_call(
        _state, name, (ulong)args.LongLength, pArgs, &outc, &outa
      );

      if (err.category != syserr_category.OK)
        throw new YumException(err);      

      for (ulong i = 0; i < outc; i++)
      {
        output.Add(Conversion.VariantToObject(outa[i]));
      }
    }

    INative.libyum_clear(_state);
    return [.. output];
  }

  public void Push(string name, object value)
  {
    var parts = name.Split('.');
    var beg = string.Join('.', parts[0..^1]);
    var end = parts[^1];

    INative.libyum_ensure_path(_state, beg);
    
    unsafe
    {
      List<byte[]> pin = [];
      variant_t variant = Conversion.ObjectToVariant(value, pin);
      INative.libyum_push_variant(_state, end, &variant);
    }

    INative.libyum_clear(_state);
  }

  public unsafe void PushCallback(string name, Func<object[], object[]> func)
  {
    variant_t* cfun(ulong argc, variant_t* argv, ulong* outc)
    {
      List<byte[]> pins = [];
      object[] cs_args = new object[argc];
      for (ulong i = 0; i < argc; i++)
      {
        cs_args[i] = Conversion.VariantToObject(argv[i]);
      } 
      
      var csout = func(cs_args);

      variant_t* cs2luaout = (variant_t*)INative.yumalloc((ulong)(sizeof(variant_t) * csout.Length));
      // KNOW ! cs3luaout is owned by YumEngine, and you MAY NOT free it.
      for (int i = 0; i < csout.Length; i++)
      {
        cs2luaout[i] = Conversion.ObjectToVariant(csout[i], pins);
      }

      *outc = (ulong)csout.Length;

      return cs2luaout;
    }

    _gpins.Add(cfun);

    INative.libyum_push_callback(_state, Ensure(name), cfun);
    INative.libyum_clear(_state);
  }

  public unsafe void PushCallback(string name, Action<object[]> action)
  {
    variant_t* cfun(ulong argc, variant_t* argv, ulong* outc)
    {
      List<byte[]> pins = [];
      object[] cs_args = new object[argc];
      for (ulong i = 0; i < argc; i++)
      {
        cs_args[i] = Conversion.VariantToObject(argv[i]);
      } 
      
      action(cs_args);
      *outc = 0;

      return null;
    }

    _gpins.Add(cfun);

    INative.libyum_push_callback(_state, Ensure(name), cfun);
    INative.libyum_clear(_state);
  }

  public void PushCallback(string name, object self, MethodInfo info)
  {
    PushCallback(name, (args) =>
    {
      return (object[])info.Invoke(self, args);
    });
  }

  public void Dispose()
  {
    if (!disposed)
    INative.libyum_delete(_state);
    disposed = true;
  }
}