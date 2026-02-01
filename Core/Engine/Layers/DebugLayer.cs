using Godot;
using System;

namespace monoe.exe.Core.Engine.Layers;

public partial class DebugLayer : Control
{
	private readonly RichTextLabel info = new();

	private long _lastAllocated;
	private double _fpsSmoothed;

	public override void _Ready()
	{
		info.SetAnchorsPreset(LayoutPreset.FullRect);
		info.CustomMinimumSize = GetWindow().Size;
		info.BbcodeEnabled = true;
		info.MouseFilter = MouseFilterEnum.Ignore;

		AddChild(info);

		SetFontSize();

		_lastAllocated = GC.GetTotalAllocatedBytes();

		EngineConsole.Verbose("debug layer ready!");
	}

	public override void _Process(double delta)
	{
		// ---- FPS ----
		double fpsRaw = Godot.Engine.GetFramesPerSecond();
		_fpsSmoothed = Mathf.Lerp(
			_fpsSmoothed == 0 ? fpsRaw : _fpsSmoothed,
			fpsRaw,
			0.1
		);

		double frameMs = delta * 1000.0;

		// ---- C# MEMORY ----
		long heapUsed = GC.GetTotalMemory(false);
		long allocatedTotal = GC.GetTotalAllocatedBytes();
		long allocThisFrame = allocatedTotal - _lastAllocated;
		_lastAllocated = allocatedTotal;

		// ---- SYSTEM MEMORY (RELEASE SAFE) ----
		var mem = OS.GetMemoryInfo();

		long physical = GetMem(mem, "physical");
		long free = GetMem(mem, "free");
		long available = GetMem(mem, "available");
		long stack = GetMem(mem, "stack");

		// ---- COLORS ----
		string fpsColor =
			_fpsSmoothed >= 120 ? "lime" :
			_fpsSmoothed >= 60  ? "green" :
			_fpsSmoothed >= 30  ? "yellow" : "red";

		string allocColor =
			allocThisFrame < 1_000   ? "lime" :
			allocThisFrame < 50_000  ? "yellow" : "red";

		info.Text =
$@"{Color($"FPS: {_fpsSmoothed:0.0}", fpsColor)}
Delta: {delta:0.0000}s
Frame Time: {frameMs:0.00} ms

{Header("C# / CLR")}
Heap Used: {FormatBytes(heapUsed)}
Allocated (lifetime): {FormatBytes(allocatedTotal)}
Alloc / Frame: {Color(FormatBytes(allocThisFrame), allocColor)}

{Header("System Memory")}
Physical RAM: {FormatBytes(physical)}
Free RAM: {FormatBytes(free)}
Available RAM: {FormatBytes(available)}
Thread Stack: {FormatBytes(stack)}";
	}

	public void SetFontSize()
	{
		info.AddThemeFontSizeOverride("normal_font_size", 35);
		info.AddThemeFontSizeOverride("bold_font_size", 35);
		info.AddThemeFontSizeOverride("bold_italic_font_size", 35);
		info.AddThemeFontSizeOverride("italic_font_size", 35);
		info.AddThemeFontSizeOverride("mono_font_size", 35);
	}

	// ---------------- HELPERS ----------------

	private static long GetMem(Godot.Collections.Dictionary dict, string key)
	{
		if (!dict.ContainsKey(key))
			return -1;

		return (long)dict[key];
	}

	private static string Header(string text)
		=> $"\n[color=cyan][b]{text}[/b][/color]";

	private static string Color(string text, string color)
		=> $"[color={color}]{text}[/color]";

	private static string FormatBytes(double bytes)
	{
		if (bytes < 0)	return "N/A";

		const double KB = 1024;
		const double MB = KB * 1024;
		const double GB = MB * 1024;

		if (bytes >= GB) return $"{bytes / GB:0.00} GB";
		if (bytes >= MB) return $"{bytes / MB:0.00} MB";
		if (bytes >= KB) return $"{bytes / KB:0.00} KB";
		return $"{bytes:0} B";
	}
}
