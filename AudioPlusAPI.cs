using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;

namespace AudioPlusAPI
{
    public class AudioPlusAPI : Mod
    {
        public static AudioPlusAPI Instance => ModContent.GetInstance<AudioPlusAPI>();

        public override object Call(params object[] args)
        {
            if (args.Length == 2 && args[0] is string command && command == "LoadOgg" && args[1] is Stream oggStream)
            {
                return LoadOgg(oggStream);
            }

            if (args.Length == 2 && args[0] is string playCommand && playCommand == "Play" && args[1] is CustomAudioTrack track)
            {
                track.Play();
                return null;
            }

            if (args.Length == 2 && args[0] is string stopCommand && stopCommand == "Stop" && args[1] is CustomAudioTrack track2)
            {
                track2.Stop();
                return null;
            }

            if (args.Length == 2 && args[0] is string pauseCommand && pauseCommand == "Pause" && args[1] is CustomAudioTrack track3)
            {
                track3.Pause();
                return null;
            }

            if (args.Length == 2 && args[0] is string resumeCommand && resumeCommand == "Resume" && args[1] is CustomAudioTrack track4)
            {
                track4.Resume();
                return null;
            }

            if (args.Length == 2 && args[0] is string getPosMsCommand && getPosMsCommand == "GetPosMs" && args[1] is CustomAudioTrack track5)
            {
                return track5.GetPosMs();
            }

            if (args.Length == 2 && args[0] is string getPosCommand && getPosCommand == "GetPos" && args[1] is CustomAudioTrack track6)
            {
                return track6.GetPos();
            }

            if (args.Length == 2 && args[0] is string getVolCommand && getVolCommand == "GetVolume" && args[1] is CustomAudioTrack track7)
            {
                return track7.GetVolume();
            }

            if (args.Length == 3 && args[0] is string setVolCommand && setVolCommand == "SetVolume" && args[1] is CustomAudioTrack track8 && args[2] is float volume)
            {
                track8.SetVolume(volume);
                return null;
            }

            if (args.Length == 3 && args[0] is string setPitchCommand && setPitchCommand == "SetPitch" && args[1] is CustomAudioTrack track9 && args[2] is float pitch)
            {
                track9.SetPitch(pitch);
                return null;
            }

            return null;
        }

        public static CustomAudioTrack LoadOgg(Stream oggStream)
        {
            using (NVorbis.VorbisReader reader = new NVorbis.VorbisReader(oggStream, true))
            {
                int sampleRate = reader.SampleRate;
                int channels = reader.Channels;
                float[] floatBuffer = new float[reader.TotalSamples * channels];
                int totalRead = reader.ReadSamples(floatBuffer, 0, floatBuffer.Length);

                byte[] pcmBuffer = new byte[totalRead * 2];
                for (int i = 0; i < totalRead; i++)
                {
                    short val = (short)(Math.Clamp(floatBuffer[i], -1f, 1f) * 32767f);
                    pcmBuffer[i * 2] = (byte)(val & 0xFF);
                    pcmBuffer[i * 2 + 1] = (byte)((val >> 8) & 0xFF);
                }

                using (MemoryStream memStream = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(memStream))
                {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(36 + pcmBuffer.Length);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((ushort)1);
                    writer.Write((ushort)channels);
                    writer.Write(sampleRate);
                    writer.Write(sampleRate * channels * 2);
                    writer.Write((ushort)(channels * 2));
                    writer.Write((ushort)16);
                    writer.Write("data".ToCharArray());
                    writer.Write(pcmBuffer.Length);
                    writer.Write(pcmBuffer);

                    memStream.Position = 0;
                    SoundEffect sound = SoundEffect.FromStream(memStream);
                    return new CustomAudioTrack(sound);
                }
            }
        }
    }

	public class CustomAudioTrack
    {
        private readonly SoundEffect _soundEffect;
        private SoundEffectInstance _instance;
        private readonly Stopwatch _timer = new Stopwatch();
        private float _volume = 1.0f;
        private float _pitch;

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = Math.Clamp(value, 0f, 1f);
                if (_instance != null)
                {
                    _instance.Volume = _volume;
                }
            }
        }

        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = Math.Clamp(value, -1f, 1f);
                if (_instance != null)
                {
                    _instance.Pitch = _pitch;
                }
            }
        }

        public CustomAudioTrack(SoundEffect soundEffect)
        {
            _soundEffect = soundEffect;
            _instance = _soundEffect.CreateInstance();
            _instance.Volume = _volume;
            _instance.Pitch = _pitch;
        }

        public void Play()
        {
            if (_instance.State == SoundState.Stopped)
            {
                _instance = _soundEffect.CreateInstance();
                _instance.Volume = _volume;
                _instance.Pitch = _pitch;
            }
            _instance.Play();
            _timer.Restart();
        }

        public void Stop()
        {
            _instance?.Stop();
            _timer.Stop();
            _timer.Reset();
        }

        public void Pause()
        {
            _instance?.Pause();
            _timer.Stop();
        }

        public void Resume()
        {
            _instance?.Resume();
            _timer.Start();
        }

        public double GetPosMs()
        {
            return _timer.Elapsed.TotalMilliseconds;
        }

        public TimeSpan GetPos()
        {
            return _timer.Elapsed;
        }
        
        public float GetVolume()
        {
            return _volume;
        }

        public float GetPitch()
        {
            return _pitch;
        }

        public void SetVolume(float volume)
        {
            Volume = volume;
        }

        public void SetPitch(float pitch)
        {
            Pitch = pitch;
        }
    }
}