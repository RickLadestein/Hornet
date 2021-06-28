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
        /// A bool which indicates whether the mesh is transparent
        /// </summary>
        public bool IsTransparent { get; set; }

        /// <summary>
        /// Creates a new instance of MeshComponent with default parameters
        /// </summary>
        public MeshComponent() : base() {
            this.Mesh = null;
            this.IsTransparent = false;
        }

        /// <summary>
        /// Retrieves a Mesh from the MeshResourceManager and sets it as the active Mesh in this component
        /// </summary>
        /// <param name="meshId">A string containing the ID of the mesh</param>
        /// <exception cref="MissingResourceException">Throws a MissingResourceException</exception>
        public void SetTargetMesh(String meshId)
        {
            Mesh result = MeshResourceManager.Instance.GetResource(meshId);
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
