using System;
using System.Collections.Generic;
using System.Text;
using HornetEngine.Ecs;
using HornetEngine.Util;
using System.Numerics;
using GlmSharp;

namespace Sandbox
{
    public class MonkeyScript : MonoScript
    {
        private vec3 rot;
        public override void Start()
        {
            rot = new vec3(5.3f, 10.9f, 15.97f);
            entity.Transform.SetOrientation(0, 0, 0);
        }

        public override void Update()
        {
            Entity bound = this.entity;
            bound.Transform.Rotate(new vec3(0, 1, 0), rot.z * Time.FrameDelta);
        }
    }
}
