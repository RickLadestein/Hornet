using System;
using System.Collections.Generic;
using System.Text;
using HornetEngine.Graphics;
using HornetEngine.Util;
using HornetEngine.Util.Exceptions;

namespace HornetEngine.Ecs
{
    public class MeshComponent : Component
    {
        /// <summary>
        /// The mesh that the MeshComponent is bound to
        /// </summary>
        public Mesh Mesh;

        /// <summary>
        /// Creates a new instance of MeshComponent with default parameters
        /// </summary>
        public MeshComponent() : base() {
            this.Mesh = null;
        }

        /// <summary>
        /// Retrieves a Mesh from the MeshResourceManager and sets it as the active Mesh in this component
        /// </summary>
        /// <param name="meshId"></param>
        /// <exception cref="Exception"></exception>
        public void SetTargetMesh(String meshId)
        {
            Mesh result = MeshResourceManager.GetInstance().GetResource(meshId);
            if(result == null)
            {
                throw new MissingResourceException(meshId);
            }
            Mesh = result;
        }


        public override string ToString()
        {
            if (this.Mesh != null)
            {
                return "Meshcomponent is bound to null";
            }
            else
            {
                return $"Meshcomponent is bound to {this.Mesh.Name}";
            }
        }
    }
}
