using GTAChaos.Effects;
using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GTAChaos.Utils
{
    public class AudioPlayer
    {
        private static readonly string folderName = "ChaosModAudio";
        public static readonly AudioPlayer INSTANCE = new AudioPlayer();
        private readonly List<AudioPlayer.Audio> queue = new List<AudioPlayer.Audio>();

        private async Task PlayNext()
        {
            if (!Config.Instance().PlayAudioForEffects)
                this.queue.Clear();
            this.queue.RemoveAll((Predicate<AudioPlayer.Audio>)(a => a.IsExpired()));
            if (this.queue.Count == 0)
                return;
            AudioPlayer.Audio audio = this.queue[0];
            audio.OnFinished += (EventHandler<EventArgs>)((sender, e) =>
           {
               this.queue.Remove(audio);
               this.PlayNext();
           });
            await Task.Run((Func<Task>)(async () => await audio.Play()));
        }

        public async Task PlayAudio(string path, bool playNow = false)
        {
            AudioPlayer.Audio audio = new AudioPlayer.Audio(path);
            if (!Config.Instance().PlayAudioSequentially | playNow)
            {
                await Task.Run((Func<Task>)(async () => await audio.Play()));
            }
            else
            {
                this.queue.Add(new AudioPlayer.Audio(path));
                if (this.queue.Count != 1)
                    return;
                await this.PlayNext();
            }
        }

        private WaveOutEvent GetWaveOutEvent() => new WaveOutEvent();

        public void SetAudioVolume(float volume)
        {
            volume = Math.Max(0.0f, Math.Min(volume, 1f));
            this.GetWaveOutEvent().Volume = volume;
        }

        public float GetAudioVolume() => this.GetWaveOutEvent().Volume;

        public void CreateAndPrintAudioFileReadme()
        {
            try
            {
                string str = Path.Combine(Directory.GetCurrentDirectory(), AudioPlayer.folderName);
                Directory.CreateDirectory(str);
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(str, "README.txt"), false))
                {
                    streamWriter.WriteLine("These are the available effects and their IDs that you can overwrite the sound clips for.");
                    streamWriter.WriteLine("The audio files can be in the following formats: .ogg, .mp3, .wav, .aac, .m4a");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("Example audio file name: effect_get_wasted.ogg");
                    streamWriter.WriteLine("_____________________________________________________________________________");
                    streamWriter.WriteLine();
                    foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.Effects.Get())
                    {
                        AbstractEffect abstractEffect = entry.item;
                        streamWriter.WriteLine(abstractEffect.GetDisplayName(DisplayNameType.UI) ?? "");
                        if (abstractEffect.GetAudioVariations() > 0)
                        {
                            for (int index = 0; index < abstractEffect.GetAudioVariations(); ++index)
                                streamWriter.WriteLine(string.Format("{0}_{1}", (object)abstractEffect.GetID(), (object)index));
                        }
                        else
                            streamWriter.WriteLine(abstractEffect.GetID() ?? "");
                        streamWriter.WriteLine();
                    }
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private class Audio
        {
            private readonly string[] supportedFormats = new string[4]
            {
        "mp3",
        "wav",
        "aac",
        "m4a"
            };

            public event EventHandler<EventArgs> OnFinished;

            private string Path { get; }

            private DateTime Expiry { get; }

            public Audio(string path)
            {
                this.Path = path;
                this.Expiry = DateTime.Now.AddSeconds(30.0);
            }

            public bool IsExpired() => this.Expiry < DateTime.Now;

            public Task Play()
            {
                if (this.IsExpired())
                {
                    this.Finish();
                    return Task.CompletedTask;
                }
                Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GTAChaos.assets.audio." + this.Path + ".ogg");
                WaveStream waveStream = (WaveStream)null;
                try
                {
                    waveStream = (WaveStream)new VorbisWaveReader(AudioPlayer.folderName + "/" + this.Path + ".ogg");
                }
                catch
                {
                }
                if (waveStream == null)
                {
                    foreach (string supportedFormat in this.supportedFormats)
                    {
                        try
                        {
                            waveStream = (WaveStream)new MediaFoundationReader(AudioPlayer.folderName + "/" + this.Path + "." + supportedFormat);
                            if (waveStream != null)
                                break;
                        }
                        catch
                        {
                        }
                    }
                }
                if (waveStream == null)
                {
                    try
                    {
                        waveStream = (WaveStream)new VorbisWaveReader(manifestResourceStream, false);
                    }
                    catch
                    {
                    }
                }
                if (waveStream == null)
                {
                    this.Finish();
                    return Task.CompletedTask;
                }
                WaveOutEvent waveOutEvent = AudioPlayer.INSTANCE.GetWaveOutEvent();
                waveOutEvent.Init((IWaveProvider)waveStream);
                waveOutEvent.PlaybackStopped += (EventHandler<StoppedEventArgs>)((sender, e) => this.Finish());
                waveOutEvent.Play();
                return Task.CompletedTask;
            }

            private void Finish()
            {
                EventHandler<EventArgs> onFinished = this.OnFinished;
                if (onFinished == null)
                    return;
                onFinished((object)this, new EventArgs());
            }
        }
    }
}
