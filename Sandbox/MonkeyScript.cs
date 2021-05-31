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
            rot = new vec3(0.0f, 10.0f, 0.0f);
            entity.Transform.Rotation = new vec3(0.0f, 0.0f, 90.0f);
        }

        public override void Update()
        {
            Entity bound = this.entity;
            bound.Transform.Rotation += rot * Time.FrameDelta;
        }
    }
}
