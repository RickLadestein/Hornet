using System;
using System.Collections.Generic;
using System.Text;
using HornetEngine.Sound;

namespace HornetEngine.Ecs
{
    public class AudioListenerComponent : Component, IDisposable
    {
        private static int instance_count = 0;
        public Listener Listener { get; private set; }

        public AudioListenerComponent()
        {
            instance_count += 1;
            Listener = new Listener();

            if(instance_count >= 2)
            {
                throw new Exception("Audio listener count cannot be greater than 1 in the entire scene");
            }
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

        public void Dispose()
        {
            instance_count -= 1;
        }
    }

    public class AudioListenerScript : MonoScript
    {
        public AudioListenerComponent audio_listener;

        public override void Start()
        {
            audio_listener.Listener.SetPosition(this.entity.Transform.Position);
            GlmSharp.vec3 rot = entity.Transform.Orientation * new GlmSharp.vec3(0.0f, 0.0f, 1.0f);
            audio_listener.Listener.SetLookingDir(rot);
        }

        public override void Update()
        {
            audio_listener.Listener.SetPosition(this.entity.Transform.Position);
            GlmSharp.vec3 rot = entity.Transform.Orientation * new GlmSharp.vec3(0.0f, 0.0f, 1.0f);
            audio_listener.Listener.SetLookingDir(rot);
        }

        
    }
}
