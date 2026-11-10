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

Pass `true` to `Play` to play the track in music mode:
```csharp
audioAPI.Call("Play", musInst, true);
```

While a music-mode track is playing, AudioPlusAPI uses a boss-priority scene effect to suppress Terraria's current music, then plays the OGG track and applies Terraria's music-volume slider to it. This lets stream-loaded tracks replace in-game music without requiring them to be registered as music assets.

#### Get timestamp:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    double timeMilliseconds = (double)audioAPI.Call("GetPosMs", musInst);
    TimeSpan time = (TimeSpan)audioAPI.Call("GetPos", musInst);
}
```

#### Set volume and pitch:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    audioAPI.Call("SetVolume", musInst, 0.5f);
    audioAPI.Call("SetPitch", musInst, 0.5f);
}
```

`LoadOgg` can be used normally for sound effects. Use `Play(track, true)` for music-mode playback; the track will replace the active in-game music at boss priority and respect Terraria's music-volume slider. `LoadOgg(stream, true)`, `LoadMusic`, and `LoadOggMusic` also mark the track as music mode, so calling `Play(track)` on those tracks enables the same behavior.

Pitch is clamped between `-1` and `1`. A positive value speeds up playback and raises its pitch; a negative value slows it down and lowers its pitch. Use `0` for normal playback speed.

> [!TIP]
> If you use this API, it is highly recommended to make it a hard dependency.
