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

        /// <summary>
        /// Function that runs the first frame when this script was added to the entity
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// Function that runs every frame update
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Function that runs every update from the Fixed Update Thread
        /// </summary>
        public abstract void FixedUpdate();

    }
}
