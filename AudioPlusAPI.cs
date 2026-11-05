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
        private bool _isPlaying;
        private float _volume = 1.0f;

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

        public CustomAudioTrack(SoundEffect soundEffect)
        {
            _soundEffect = soundEffect;
            _instance = _soundEffect.CreateInstance();
            _instance.Volume = _volume;
        }

        public void Play()
        {
            if (_instance.State == SoundState.Stopped)
            {
                _instance = _soundEffect.CreateInstance();
                _instance.Volume = _volume;
            }
            _instance.Play();
            _timer.Restart();
            _isPlaying = true;
        }

        public void Stop()
        {
            _instance?.Stop();
            _timer.Stop();
            _timer.Reset();
            _isPlaying = false;
        }

        public void Pause()
        {
            _instance?.Pause();
            _timer.Stop();
            _isPlaying = false;
        }

        public void Resume()
        {
            _instance?.Resume();
            _timer.Start();
            _isPlaying = true;
        }

        public double GetPositionMilliseconds()
        {
            if (!_isPlaying)
                return 0;

            return _timer.Elapsed.TotalMilliseconds;
        }

        public TimeSpan GetPosition()
        {
            return _timer.Elapsed;
        }
    }
}