using System;
using System.Collections.Generic;
using System.Text;
using HornetEngine.Ecs;
using HornetEngine.Util;
using System.Numerics;
namespace Sandbox
{
    public class MonkeyScript : MonoScript
    {
        private Vector3 rot;
        public override void Start()
        {
            rot = new Vector3(0.0f, 10.0f, 0.0f);
        }

        public override void Update()
        {
            Entity bound = this.entity;
            bound.Transform.Rotation += rot * Time.FrameDelta;
        }
    }
}
