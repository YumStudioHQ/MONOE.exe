using Godot;
using System;

namespace monoe.exe.Core.Engine.Layers;

public partial class DebugLayer : Control
{
	private Label info = new(){};

	public override void _Ready()
	{
		info.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(info);
		EngineConsole.Verbose("diagnostic layers are ready!");
		SetFontSize(60);
	}

	public void SetFontSize(long size) => info.AddThemeFontSizeOverride("font_size", (int)size);

	public override void _Process(double delta)
	{
		double fps = Performance.GetMonitor(Performance.Monitor.TimeFps);

		double godotStaticMem =	Performance.GetMonitor(Performance.Monitor.MemoryStatic);

		long dotnetMem = GC.GetTotalMemory(false);

		double processTime =	Performance.GetMonitor(Performance.Monitor.TimeProcess);

		info.Text =
$@"FPS: {fps:0.00}
Delta: {delta:0.0000}
Internals Memory: {FormatBytes(godotStaticMem)}
C# Heap: {FormatBytes(dotnetMem)}
Total: {FormatBytes(godotStaticMem + dotnetMem)}
Process Time: {processTime * 1000:0.00} ms";
	}

	private static string FormatBytes(double bytes)
	{
		const double KB = 1024;
		const double MB = KB * 1024;
		const double GB = MB * 1024;

		if (bytes >= GB) return $"{bytes / GB:0.00} GB";
		if (bytes >= MB) return $"{bytes / MB:0.00} MB";
		if (bytes >= KB) return $"{bytes / KB:0.00} KB";
		return $"{bytes:0} B";
	}
}
