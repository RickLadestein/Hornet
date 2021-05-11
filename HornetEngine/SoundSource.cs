using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using OpenTK.Audio.OpenAL;

namespace HornetEngine
{
    /// <summary>
    /// The abstract class SoundSource will be used to pass around a shared source.
    /// 
    /// This ensures that the source wont have to be re-created and deleted with every sound sample.
    /// </summary>
    public abstract class SoundSource
    {
        private int source;
        private float pitch = 1.0f;
        private float volume = 1.0f;

        /// <summary>
        /// The constructor of the Sound Source
        /// </summary>
        /// <param name="looping">A value containing true/false depending on whether the sound should loop.</param>
        public SoundSource(bool looping)
        {
            // Initialize the default values
            source = AL.GenSource();
            AL.Source(source, ALSourcef.Gain, this.volume);
            AL.Source(source, ALSourcef.Pitch, this.pitch);
            AL.Source(source, ALSourceb.Looping, looping);
            AL.Source(source, ALSource3f.Position, 0.0f, 0.0f, 0.0f);
            AL.Source(source, ALSource3f.Velocity, 0.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// A function which can be used to set the pitch of the source
        /// </summary>
        /// <param name="newPitch">The new value which should be used for the pitch.</param>
        public void setPitch(float newPitch)
        {
            this.pitch = newPitch;
            AL.Source(source, ALSourcef.Pitch, this.pitch);
        }

        /// <summary>
        /// A function which can be used to set the volume of the source
        /// </summary>
        /// <param name="newVolume">The new value which should be used for the volume.</param>
        public void setVolume(float newVolume)
        {
            this.volume = newVolume;
            AL.Source(source, ALSourcef.Gain, this.volume);
        } 

        /// <summary>
        /// A function which will return the current source
        /// </summary>
        /// <returns>The source which should be used for the sound playback.</returns>
        public int getSource() { return source; }
    }
}
