using HornetEngine.Graphics;
using HornetEngine.Graphics.Buffers;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    public class RenderComponent : Component
    {
        public RenderComponent() {
        
        }

        public void Render(Camera cam)
        {
            MaterialComponent matcomp = parent.GetComponent<MaterialComponent>();
            MeshComponent meshcomp = parent.GetComponent<MeshComponent>();
            TextureComponent texcomp = parent.GetComponent<TextureComponent>();

            if (meshcomp.Mesh == null || matcomp.Shader == null)
            {
                throw new Exception($"Could not render mesh for entity {this.parent.Id}: Mesh or Shader was null");
            }

            if (texcomp != null)
            {
                texcomp.Textures.Bind();
                for (int i = 0; i < texcomp.Textures.textures.Length; i++)
                {
                    if (texcomp.Textures.textures[i] != null)
                    {
                        matcomp.Shader.SetUniform($"texture_{i}", i);
                    }
                }
            }

            VertexBuffer vbuf = meshcomp.Mesh.VertexBuffer;
            vbuf.Bind();

            ShaderProgram sh = matcomp.Shader;
            sh.Bind();
            sh.SetUniform("model", parent.Transform.ModelMat);
            sh.SetUniform("normal_mat", parent.Transform.NormalMat);
            sh.SetUniform("projection", cam.ProjectionMatrix);
            sh.SetUniform("view", cam.ViewMatrix);
            sh.SetUniform("camera_position", cam.Position);
            sh.SetUniform("camera_target", cam.Target);
            sh.SetUniform("time", (float)NativeWindow.GLFW.GetTime());

            NativeWindow.GL.DrawArrays((GLEnum)vbuf.PrimitiveType, 0, vbuf.VertexCount);
            vbuf.Unbind();
            ShaderProgram.UnbindAll();

            if (texcomp != null)
            {
                texcomp.Textures.Unbind();
            }
        }
        public override string ToString()
        {
            return "Rendercomponent: {\n" +
                $"\tparent: Entity[{this.parent.Id}]\n" +
                "}";
        }
    }
}
