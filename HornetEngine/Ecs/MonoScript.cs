using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    public abstract class MonoScript
    {
        public Entity entity;


        public MonoScript() {
            this.Start();
        }
        public abstract void Start();

        public abstract void Update();

    }
}
