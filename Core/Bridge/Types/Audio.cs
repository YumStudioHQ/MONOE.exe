using System.IO;
using Godot;

namespace monoe.exe.Core.Bridge.Types;

public class Audio : Exposable
{
  private readonly AudioStreamPlayer audio = new();
  private bool isEventAdded = false;

  public void Load(string path, string fmt)
  {
    var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read) ?? throw new FileNotFoundException($"cannot open file {path}");

    var buff = file.GetBuffer((long)file.GetLength());

    audio.Stream = fmt switch
    {
      "wav" => new AudioStreamWav() { Data = buff },
      "mp3" => new AudioStreamMP3() { Data = buff },
      _ => throw new InvalidDataException($"unsupported format {fmt}"),
    };
  }

  public void Play(double at = 0.0, bool loop = false)
  {
    audio.Play((float)at);
    if (audio.Stream is AudioStreamWav wav)
      wav.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
    else if (audio.Stream is AudioStreamMP3 mP3)
      mP3.Loop = loop;
  }

  public void Stop() => audio.Stop();
  public double Length() => audio.Stream.GetLength();
  
  public string FinishedEvent()
  {
    string @event = $"_on_audio#{UID}_finished";

    if (isEventAdded) audio.Finished += () =>
    {
      Main.Emit(@event);
    };

    isEventAdded = true;
    return @event;
  }

  public override Node NRef() => audio;

  protected override void _Free()
  {
    audio.QueueFree();
  }
}