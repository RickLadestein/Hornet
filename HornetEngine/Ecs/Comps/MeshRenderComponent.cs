using HornetEngine.Graphics;
using HornetEngine.Graphics.Buffers;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    public class MeshRenderComponent : Component
    {
        private MaterialComponent default_material;
        public MeshRenderComponent()
        {
            default_material = new MaterialComponent();
            default_material.SetShaderFromId("default");
            default_material.SetTextureUnit("default", HTextureUnit.Unit_0);
        }

        public void Render(Camera target)
        {
            

            if (!parent.HasComponent<MeshComponent>())
            {
                throw new Exception("Tried to draw entity without a mesh on screen");
            }

            MeshComponent meshcomp = parent.GetComponent<MeshComponent>();
            MaterialComponent matcomp = parent.GetComponent<MaterialComponent>();
            if (matcomp == null)
            {
                matcomp = default_material;
            }
            ShaderProgram sh = matcomp.Shader;
            sh.Bind();
            
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
            sh.SetUniform("model", parent.Transform.ModelMat);
            sh.SetUniform("normal_mat", parent.Transform.NormalMat);
            sh.SetUniform("projection", target.ProjectionMatrix);
            sh.SetUniform("view", target.ViewMatrix);
            sh.SetUniform("time", (float)NativeWindow.GLFW.GetTime());

            NativeWindow.GL.DrawArrays((GLEnum)vbuf.PrimitiveType, 0, vbuf.VertexCount);
            vbuf.Unbind();
            ShaderProgram.UnbindAll();
            matcomp.Textures.Unbind();
            if (matcomp != null)
            {
                matcomp.Textures.Unbind();
            }
        }

        public static void RenderOnCamPlane(Camera cam)
        {
            MaterialComponent matcomp = cam.Material;
            MeshComponent meshcomp = cam.Render_plane;

            ShaderProgram sh = matcomp.Shader;
            sh.Bind();

            cam.FrameBuffer.Textures.Bind();
            for (int i = 0; i < cam.FrameBuffer.Textures.textures.Length; i++)
            {
                if (cam.FrameBuffer.Textures.textures[i] != null)
                {
                    matcomp.Shader.SetUniform($"texture_{i}", i);
                }
            }

            VertexBuffer vbuf = meshcomp.Mesh.VertexBuffer;
            vbuf.Bind();
            sh.SetUniform("time", (float)NativeWindow.GLFW.GetTime());

            NativeWindow.GL.DrawArrays((GLEnum)vbuf.PrimitiveType, 0, vbuf.VertexCount);
            vbuf.Unbind();
            ShaderProgram.UnbindAll();
            cam.FrameBuffer.Textures.Unbind();
        }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
