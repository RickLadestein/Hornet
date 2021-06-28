using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    public abstract class MonoScript
    {
        /// <summary>
        /// The entity within the MonoScript
        /// </summary>
        public Entity entity;

        /// <summary>
        /// A bool used to check whether the script has been run
        /// </summary>
        public bool start;

        /// <summary>
        /// The constructor of the MonoScript
        /// </summary>
        public MonoScript() {
            start = true;
        }

        /// <summary>
        /// Function that runs the first frame when this script was added to the entity
        /// </summary>
        public virtual void Start() {
            start = false;
        }

        /// <summary>
        /// Function that runs every frame update
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Function that runs every update from the Fixed Update Thread
        /// </summary>
        public virtual void FixedUpdate() { }
    }
}
