using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using HornetEngine.Sound;
namespace HornetEngine.Ecs
{
    public class SoundSourceComponent : Component
    {
        /// <summary>
        /// A list of active SoundSources
        /// </summary>
        public List<SoundSource> ActiveSources;

        private System.Timers.Timer source_refresh_timer;
        private Mutex src_mutex;

        /// <summary>
        /// The constructor of the SoundSourceComponent
        /// </summary>
        public SoundSourceComponent()
        {
            ActiveSources = new List<SoundSource>();
            src_mutex = new Mutex();
            source_refresh_timer = new System.Timers.Timer();
            source_refresh_timer.Interval = 500;
            source_refresh_timer.Elapsed += Source_refresh_timer_Elapsed;
            source_refresh_timer.Start();
        }

        public bool IsPlaying
        {
            get
            {
                int playing = this.ActiveSources.Count;
                return playing > 0;
            }
        }

        /// <summary>
        /// Plays a sound effect once with pitch and volume
        /// </summary>
        /// <param name="samp">The sample to be played</param>
        /// <param name="pitch">The playback pitch [0.5 > pitch < float.maxvalue]</param>
        /// <param name="volume">The source volume at which the sound effect plays</param>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException</exception>
        public void PlaySoundEffect(Sample samp, float pitch, float volume)
        {
            if(samp == null)
            {
                throw new ArgumentNullException("Sample");
            }

            SoundSource src = new SoundSource(false);
            src.SetPitch(pitch);
            src.SetVolume(volume);
            src.SetPosition(parent.Transform.Position);
            src.PlaySoundEffect(samp);

            src_mutex.WaitOne();
            ActiveSources.Add(src);
            src_mutex.ReleaseMutex();
        }

        /// <summary>
        /// Checks whether the SoundSourceComponent is already playing a sound
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                int playing = this.ActiveSources.Count;
                return playing > 0;
            }
        }

        /// <summary>
        /// A function which can be used to play music
        /// </summary>
        /// <param name="samp">The sample which should be played</param>
        /// <param name="pitch">The pitch which should be used</param>
        /// <param name="volume">The volume which should be used</param>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException</exception>
        public void PlayMusic(Sample samp, float pitch, float volume)
        {
            if (samp == null)
            {
                throw new ArgumentNullException("Sample");
            }

            SoundSource src = new SoundSource(false);
            src.SetPitch(pitch);
            src.SetVolume(volume);
            src.PlaySoundEffect(samp);

            src_mutex.WaitOne();
            ActiveSources.Add(src);
            src_mutex.ReleaseMutex();
        }

        private void Source_refresh_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            src_mutex.WaitOne();
            ActiveSources.RemoveAll(src => {
                return src.GetState() == OpenTK.Audio.OpenAL.ALSourceState.Stopped;
            });
            src_mutex.ReleaseMutex();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
