using System;
using Godot;

namespace LudumDareApril2026.Core;

/// <summary>
/// Lightweight autoload for playing SFX and music from anywhere in the game.
/// Dedicated <see cref="AudioStreamPlayer"/> children keep music and one-shot
/// SFX independent.
/// </summary>
public partial class AudioManager : Node
{
    private static AudioManager? _instance;
    public static AudioManager Instance =>
        _instance ?? throw new InvalidOperationException(
            "AudioManager autoload not initialized. Check project.godot autoload config.");

    private AudioStreamPlayer _sfxPlayer = null!;
    private AudioStreamPlayer _musicPlayer = null!;

    public override void _EnterTree()
    {
        if (_instance is not null && _instance != this)
        {
            QueueFree();
            return;
        }
        _instance = this;
    }

    public override void _Ready()
    {
        _sfxPlayer = new AudioStreamPlayer
        {
            Name = "SfxPlayer",
            Bus = "Master",
        };
        _musicPlayer = new AudioStreamPlayer
        {
            Name = "MusicPlayer",
            Bus = "Master",
        };
        AddChild(_sfxPlayer);
        AddChild(_musicPlayer);
    }

    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void PlaySfx(AudioStream? stream, float volumeDb = 0f)
    {
        if (stream is null) return;
        _sfxPlayer.Stream = stream;
        _sfxPlayer.VolumeDb = volumeDb;
        _sfxPlayer.Play();
    }

    public void PlayMusic(AudioStream? stream, float volumeDb = -6f, bool loop = true)
    {
        if (stream is null)
        {
            _musicPlayer.Stop();
            return;
        }

        if (_musicPlayer.Stream == stream && _musicPlayer.Playing)
        {
            return;
        }

        _musicPlayer.Stream = stream;
        _musicPlayer.VolumeDb = volumeDb;

        if (stream is AudioStreamOggVorbis ogg)
        {
            ogg.Loop = loop;
        }

        _musicPlayer.Play();
    }

    public void StopMusic() => _musicPlayer.Stop();

    public void SetMasterVolumeLinear(float linear)
    {
        var busIndex = AudioServer.GetBusIndex("Master");
        if (busIndex < 0) return;
        AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(Mathf.Clamp(linear, 0.0001f, 1f)));
    }

    public float GetMasterVolumeLinear()
    {
        var busIndex = AudioServer.GetBusIndex("Master");
        if (busIndex < 0) return 1f;
        return Mathf.DbToLinear(AudioServer.GetBusVolumeDb(busIndex));
    }
}
