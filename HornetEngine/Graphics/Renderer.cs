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

        public void InitFrameBuffer(Window window)
        {
            GlmSharp.vec2 size = window.Size;
            fb = new FrameBuffer((uint)size.x, (uint)size.y);
        }
        public void RenderEntity(Camera cam, Entity entity)
        {
            if(!entity.HasComponent<MeshComponent>())
            {
                throw new Exception("Tried to draw entity without a mesh on screen");
            }

            MeshComponent meshcomp = entity.GetComponent<MeshComponent>();
            MaterialComponent matcomp = entity.GetComponent<MaterialComponent>();
            if(matcomp == null)
            {
                matcomp = default_material;
            }

            matcomp.Textures.Bind();
            for (int i = 0; i < matcomp.Textures.textures.Length; i++)
            {
                if (matcomp.Textures.textures[i] != null)
                {
                    matcomp.Shader.SetUniform($"texture_{i}", i);
                }
            }

            VertexBuffer vbuf = meshcomp.Mesh.VertexBuffer;
            vbuf.Bind();

            ShaderProgram sh = matcomp.Shader;
            sh.Bind();
            sh.SetUniform("model", entity.Transform.ModelMat);
            sh.SetUniform("normal_mat", entity.Transform.NormalMat);
            sh.SetUniform("projection", cam.ProjectionMatrix);
            sh.SetUniform("view", cam.ViewMatrix);
            sh.SetUniform("camera_position", cam.Position);
            sh.SetUniform("camera_target", cam.Target);
            sh.SetUniform("time", (float)NativeWindow.GLFW.GetTime());

            NativeWindow.GL.DrawArrays((GLEnum)vbuf.PrimitiveType, 0, vbuf.VertexCount);
            vbuf.Unbind();
            ShaderProgram.UnbindAll();

            if (matcomp != null)
            {
                matcomp.Textures.Unbind();
            }
        }
    }
}
