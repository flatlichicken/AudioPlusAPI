### What this is for:
I made this API for audio systems that need to be precice.

### Examples for how to use code:

#### Load audio:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    using (Stream s = Mod.GetFileStream("Content/Charts/Music/Inst.ogg")) 
        musInst = audioAPI.Call("LoadOgg", s);
}
```

#### Load music instead of a sound effect:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    using (Stream s = Mod.GetFileStream("Content/Charts/Music/Theme.ogg"))
        musTheme = audioAPI.Call("LoadOgg", s, true);

    // or:
    // using (Stream s = Mod.GetFileStream("Content/Charts/Music/Theme.ogg"))
    //     musTheme = audioAPI.Call("LoadMusic", s);
}
```

#### Play/Stop/Pause/Resume audio:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    audioAPI.Call("Play", musInst);
    audioAPI.Call("Stop", musInst);
    audioAPI.Call("Pause", musInst);
    audioAPI.Call("Resume", musInst);
}
```

#### Play with override mode:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    audioAPI.Call("Play", musInst, true);
}
```

While an override-mode sound is playing, AudioPlusAPI uses a boss-priority scene effect to suppress Terraria's current music, then plays the OGG track and applies Terraria's music-volume slider to it. This lets stream-loaded tracks replace in-game music without requiring them to be registered as music assets.

#### Play with a specific volume mode:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    audioAPI.Call("Play", musInst, "music");
    audioAPI.Call("Play", musInst, "sound");
    audioAPI.Call("Play", musInst, "ambient");
    // you can leave the last argument blank if you want so it can be unaffected by the volume sliders.
}
```

#### Play with combined volume mode and override mode:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    audioAPI.Call("Play", musInst, "music", true);
}
```

#### Get timestamp and full length:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    double timeMilliseconds = (double)audioAPI.Call("GetPosMs", musInst);
    double totalMilliseconds = (double)audioAPI.Call("GetLengthMs", musInst);
    TimeSpan time = (TimeSpan)audioAPI.Call("GetPos", musInst);
    float localVolume = (float)audioAPI.Call("GetVolume", musInst);
}
```

#### Set volume and pitch:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    audioAPI.Call("SetVolume", musInst, 0.5f);
    audioAPI.Call("SetPitch", musInst, 0.5f);
    audioAPI.Call("SetLoop", musInst, true);
}
```

Pitch is clamped between `-1` and `1`. A positive value speeds up playback and raises its pitch; a negative value slows it down and lowers its pitch. Use `0` for normal playback speed.
Set looping on a music track with `SetLoop`; pass `false` to disable it.

> [!TIP]
> If you use this API, it is highly recommended to make it a hard dependency.
