using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace monoe.exe.YumSharp.Natives;

internal static unsafe partial class INative
{
#if WINDOWS
    private const string LibName = "yum.dll";
#elif LINUX
    private const string LibName = "yum.so";
#elif OSX || GODOT_MACOS || GODOT_OSX
    private const string LibName = "libyum_apple.dylib";
#else
    private const string LibName = "yum";
#endif

    private const string DllName = $"libs/{LibName}";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate variant_t* YumCallback(
        ulong argc,
        variant_t* argv,
        ulong* outc
    );

    [LibraryImport(DllName, EntryPoint = "libyum_new")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr libyum_new();

    [LibraryImport(DllName, EntryPoint = "libyum_delete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void libyum_delete(IntPtr state);

    [LibraryImport(DllName, EntryPoint = "libyum_open_libs")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void libyum_open_libs(IntPtr state);

    [LibraryImport(DllName, EntryPoint = "libyum_push_callback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial syserr_t libyum_push_callback(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
        YumCallback callback
    );

    [LibraryImport(DllName, EntryPoint = "libyum_call")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial syserr_t libyum_call(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string path,
        ulong argc,
        variant_t* argv,
        ulong* outc,
        variant_t** @out
    );

    [LibraryImport(DllName, EntryPoint = "libyum_push_variant")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void libyum_push_variant(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
        variant_t* var
    );

    [LibraryImport(DllName, EntryPoint = "libyum_push_table")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void libyum_push_table(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name
    );

    [LibraryImport(DllName, EntryPoint = "libyum_new_table")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void libyum_new_table(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name
    );

    [LibraryImport(DllName, EntryPoint = "libyum_push_global")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void libyum_push_global(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name
    );

    [LibraryImport(DllName, EntryPoint = "libyum_run")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial syserr_t libyum_run(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string source,
        sbyte isfile
    );

    [LibraryImport(DllName, EntryPoint = "libyum_load")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial syserr_t libyum_load(
        IntPtr state,
        lstring_t* source,
        sbyte isfile
    );

    [LibraryImport(DllName, EntryPoint = "libyum_clear")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void libyum_clear(IntPtr state);
    

    [LibraryImport(DllName, EntryPoint = "libyum_ensure_path")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void libyum_ensure_path(IntPtr state, [MarshalAs(UnmanagedType.LPUTF8Str)] string path);


    [LibraryImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static partial lstring_t yumfmterr(syserr_t err); 
    
    [LibraryImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static partial IntPtr yumalloc(ulong bytes); 
    
    [LibraryImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static partial void yumfree(IntPtr p);

    [LibraryImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static partial void yumfree_array(variant_t *p, ulong length);
    
    [LibraryImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static partial void yumfree_all(variant_t *p, ulong length);
}
