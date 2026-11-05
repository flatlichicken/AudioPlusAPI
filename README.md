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

#### Get timestamp:
```csharp
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
    double timeMilliseconds = (double)audioAPI.Call("GetPosMs", musInst);
    TimeSpan time = (TimeSpan)audioAPI.Call("GetPos", musInst);
}
```

> [!TIP]
> If you use this API, it is highly recommended to make it a hard dependency.
