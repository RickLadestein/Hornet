using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    public abstract class Component
    {
        public Guid id { get; private set; }
        public Component()
        {
            id = Guid.NewGuid();
        }

        public abstract new String ToString();
    }
}
