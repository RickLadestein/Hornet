using System;
using System.Collections.Generic;
using System.Text;
using HornetEngine.Sound;

namespace HornetEngine.Ecs
{
    public class AudioListenerComponent : Component, IDisposable
    {
        private static int instance_count = 0;

        /// <summary>
        /// The listener
        /// </summary>
        public Listener Listener { get; private set; }

        /// <summary>
        /// The constructor of the AudioListenerComponent
        /// </summary>
        /// <exception cref="Exception">Throws an Exception</exception>
        public AudioListenerComponent()
        {
            instance_count += 1;
            Listener = new Listener();

            if(instance_count >= 2)
            {
                throw new Exception("Audio listener count cannot be greater than 1 in the entire scene");
            }
            Listener.setGlobalVol(0.5f);
        }

        private new void Initialise()
        {
            if(!this.parent.HasScript<AudioListenerScript>())
            {
                AudioListenerScript scr = new AudioListenerScript();
                scr.audio_listener = this;
                this.parent.AddScript(scr);
            }
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A function which can be used to dispose of the component
        /// </summary>
        public void Dispose()
        {
            instance_count -= 1;
        }
    }

    public class AudioListenerScript : MonoScript
    {
        /// <summary>
        /// An AudioListenerComponent used within the script
        /// </summary>
        public AudioListenerComponent audio_listener;

        /// <summary>
        /// A function which can be used to start the audio listener
        /// </summary>
        public override void Start()
        {
            audio_listener.Listener.SetPosition(this.entity.Transform.Position);
            GlmSharp.vec3 rot = entity.Transform.Orientation * new GlmSharp.vec3(0.0f, 0.0f, 1.0f);
            audio_listener.Listener.SetLookingDir(rot);
        }

        /// <summary>
        /// A function which can be used to update the audio listener
        /// </summary>
        public override void Update()
        {
            audio_listener.Listener.SetPosition(this.entity.Transform.Position);
            GlmSharp.vec3 rot = entity.Transform.Orientation * new GlmSharp.vec3(0.0f, 0.0f, 1.0f);
            audio_listener.Listener.SetLookingDir(rot);
        }
    }
}
