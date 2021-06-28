using HornetEngine.Ecs;
using HornetEngine.Graphics.Buffers;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Graphics
{
    public class Renderer
    {
        private static Renderer _instance;
        private static object lck = new object();

        /// <summary>
        /// The instance of the renderer
        /// </summary>
        public static Renderer Instance
        {
            get
            {
                lock (lck)
                {
                    if (_instance == null)
                    {
                        _instance = new Renderer();
                    }
                }
                return _instance;
            }
        }

        private FrameBuffer fb;
        private MaterialComponent default_material;

        private Renderer()
        {
            default_material = new MaterialComponent();
            default_material.SetShaderFromId("default");
            default_material.SetTextureUnit("default", HTextureUnit.Unit_0);
        }

        /// <summary>
        /// A function which renders the entity
        /// </summary>
        /// <param name="cam">The given camera</param>
        /// <param name="entity">The given entity</param>
        public void RenderEntity(Camera cam, Entity entity)
        {

        }
    }
}
