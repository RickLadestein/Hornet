using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using OpenTK.Audio.OpenAL;

namespace HornetEngine.Sound
{
    /// <summary>
    /// The abstract class SoundSource will be used to pass around a shared source.
    /// 
    /// This ensures that the source wont have to be re-created and deleted with every sound sample.
    /// </summary>
    public class SoundSource
    {
        private int handle;
        private float pitch = 1.0f;
        private float volume = 1.0f;

        /// <summary>
        /// The constructor of the Sound Source
        /// </summary>
        /// <param name="looping">A value containing true/false depending on whether the sound should loop.</param>
        public SoundSource(bool looping)
        {
            // Initialize the default values
            handle = AL.GenSource();
            AL.Source(handle, ALSourcef.Gain, this.volume);
            AL.Source(handle, ALSourcef.Pitch, this.pitch);
            AL.Source(handle, ALSourceb.Looping, looping);
            AL.Source(handle, ALSource3f.Position, 0.0f, 0.0f, 0.0f);
            AL.Source(handle, ALSource3f.Velocity, 0.0f, 0.0f, 0.0f);
        }

        ~SoundSource()
        {
            AL.DeleteSource(this.handle);
        }

        /// <summary>
        /// A function which will play a sound effect
        /// </summary>
        /// <param name="s">The sample which should be played</param>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException</exception>
        public void PlaySoundEffect(Sample s)
        {
            if(s == null)
            {
                throw new ArgumentNullException("Sample");
            }
            AL.Source(this.handle, ALSourcei.Buffer, s.Handle);
            AL.SourcePlay(this.handle);
        }

        /// <summary>
        /// A function which gets the current ALSourceState
        /// </summary>
        /// <returns>Returns the current state</returns>
        public ALSourceState GetState()
        {
            AL.GetSource(this.handle, ALGetSourcei.SourceState, out int state);
            return (ALSourceState)state;
        }

        /// <summary>
        /// A function which can be used to pause a sound
        /// </summary>
        public void Pause()
        {
            if(GetState() == ALSourceState.Playing)
            {
                AL.SourcePause(this.handle);
            }
        }

        /// <summary>
        /// A function which can be used to stop a sound
        /// </summary>
        public void Stop()
        {
            ALSourceState state = GetState();
            if (state == ALSourceState.Playing || state == ALSourceState.Paused)
            {
                AL.SourceStop(this.handle);
            }
        }

        /// <summary>
        /// A function which can be used to set the pitch of the source
        /// </summary>
        /// <param name="newPitch">The new value which should be used for the pitch.</param>
        public void SetPitch(float newPitch)
        {
            this.pitch = newPitch;
            AL.Source(handle, ALSourcef.Pitch, this.pitch);
        }

        /// <summary>
        /// A function which can be used to set the volume of the source
        /// </summary>
        /// <param name="newVolume">The new value which should be used for the volume.</param>
        public void SetVolume(float newVolume)
        {
            this.volume = newVolume;
            AL.Source(handle, ALSourcef.Gain, this.volume);
        } 

        /// <summary>
        /// A function which can be used to set the position of the sound source
        /// </summary>
        /// <param name="world_pos">A vec3 containing the new position</param>
        public void SetPosition(GlmSharp.vec3 world_pos)
        {
            AL.Source(handle, ALSource3f.Position, world_pos.x, world_pos.y, world_pos.z);
        }

        /// <summary>
        /// A function which can be used to set the velocity
        /// </summary>
        /// <param name="velocity">A vec3 containing the new velocity</param>
        public void SetVelocity(GlmSharp.vec3 velocity)
        {
            AL.Source(handle, ALSource3f.Velocity, velocity.x, velocity.y, velocity.z);
        }
    }
}
