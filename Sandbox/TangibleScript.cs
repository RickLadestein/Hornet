using HornetEngine.Ecs;
using HornetEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Text;


namespace Sandbox
{
    public class TangibleScript : MonoScript
    {
        public Camera cam;
        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();

            //TangibleObjects[] found = Touchmanager.Instance.GetTangibleObjects()
            // switch(found[0].type) 
                //case TangibleType.Alpha:
                //sound_source.play("gitaar");
                //break
            //cam.Position += new GlmSharp.vec3(1.0f, 0.0f, 0.0f);

        }
    }
}
