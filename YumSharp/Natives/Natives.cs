using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
namespace monoe.exe.YumSharp.Natives;

internal static unsafe partial class INative
{
#if GODOT_MACOS
    private const string DllName = $"libs/libyum_apple";
#else
    #if GODOT_WINDOWS || WINDOWS
        private const string DllPlatform = "win";
        #warning windows
    #else
        private const string DllPlatform = "linux";
        #warning linux
    #endif

    #if X64
        private const string DllArch = "x64";
        #warning 64
    #elif X86
        private const string DllArch = "x86";
        #warning 86
    #else
        private const string DllArch = "arm64";
        #warning ARM
    #endif

    private const string DllName = $"libs/libyum_{DllPlatform}_{DllArch}";
#endif

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate variant_t* YumCallback(
        ulong argc,
        variant_t* argv,
        ulong* outc
    );

    [DllImport(DllName, EntryPoint = "libyum_new")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern IntPtr libyum_new();

    [DllImport(DllName, EntryPoint = "libyum_delete")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern void libyum_delete(IntPtr state);

    [DllImport(DllName, EntryPoint = "libyum_open_libs")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern void libyum_open_libs(IntPtr state);

    [DllImport(DllName, EntryPoint = "libyum_push_callback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern syserr_t libyum_push_callback(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
        YumCallback callback
    );

    [DllImport(DllName, EntryPoint = "libyum_call")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern syserr_t libyum_call(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string path,
        ulong argc,
        variant_t* argv,
        ulong* outc,
        variant_t** @out
    );

    [DllImport(DllName, EntryPoint = "libyum_push_variant")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern void libyum_push_variant(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
        variant_t* var
    );

    [DllImport(DllName, EntryPoint = "libyum_push_table")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern void libyum_push_table(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name
    );

    [DllImport(DllName, EntryPoint = "libyum_new_table")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern void libyum_new_table(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name
    );

    [DllImport(DllName, EntryPoint = "libyum_push_global")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern void libyum_push_global(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name
    );

    [DllImport(DllName, EntryPoint = "libyum_run")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern syserr_t libyum_run(
        IntPtr state,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string source,
        sbyte isfile
    );

    [DllImport(DllName, EntryPoint = "libyum_load")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern syserr_t libyum_load(
        IntPtr state,
        lstring_t* source,
        sbyte isfile
    );

    [DllImport(DllName, EntryPoint = "libyum_clear")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern void libyum_clear(IntPtr state);
    

    [DllImport(DllName, EntryPoint = "libyum_ensure_path")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static extern void libyum_ensure_path(IntPtr state, [MarshalAs(UnmanagedType.LPUTF8Str)] string path);


    [DllImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static extern lstring_t yumfmterr(syserr_t err); 
    
    [DllImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static extern IntPtr yumalloc(ulong bytes); 
    
    [DllImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static extern void yumfree(IntPtr p);

    [DllImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static extern void yumfree_array(variant_t *p, ulong length);
    
    [DllImport(DllName)][UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])] 
    public static extern void yumfree_all(variant_t *p, ulong length);
}
