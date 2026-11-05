examples for how to use code:

Load audio:
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
  using (Stream s = Mod.GetFileStream("Content/Charts/Music/Inst.ogg")) musInst = audioAPI.Call("LoadOgg", s);
}

Play/Pause/Stop audio:
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
  audioAPI.Call("Play", musInst);
  audioAPI.Call("Stop", musInst);
  audioAPI.Call("Pause", musInst);
  audioAPI.Call("Resume", musInst);
}

get timestamp:
if (ModLoader.TryGetMod("AudioPlusAPI", out Mod audioAPI))
{
  double timeMilliseconds = (double)audioAPI.Call("GetPosMs", musInst);
  TimeSpan time = (TimeSpan)audioAPI.Call("GetPos", musInst);
}

if you use this api, its recomended to make it a hard dependency
