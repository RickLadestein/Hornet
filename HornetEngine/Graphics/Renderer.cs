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


        public void RenderEntity(Camera cam, Entity entity)
        {
            
        }
    }
}
