using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    public abstract class MonoScript
    {
        public Entity self;

        public abstract void Start();

        public abstract void Update();

    }
}
