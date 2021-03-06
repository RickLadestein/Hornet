using HornetEngine.Graphics;
using HornetEngine.Graphics.Buffers;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    class ForewardRenderComponent : Component
    {
        private MaterialComponent default_material;

        /// <summary>
        /// The constructor of the ForewardRenderComponent
        /// </summary>
        public ForewardRenderComponent()
        {
            default_material = new MaterialComponent();
            default_material.SetShaderFromId("default");
            default_material.SetTextureUnit("default", HTextureUnit.Unit_0);
        }

        /// <summary>
        /// A function which can be used to render the target
        /// </summary>
        /// <param name="target">The camera</param>
        /// <exception cref="Exception">Throws an Exception</exception>
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
            sh.SetUniform("model", parent.Transform.ModelMat);
            sh.SetUniform("normal_mat", parent.Transform.NormalMat);
            sh.SetUniform("projection", target.ProjectionMatrix);
            sh.SetUniform("view", target.ViewMatrix);
            sh.SetUniform("camera_position", target.Position);
            sh.SetUniform("camera_target", target.Target);
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
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
