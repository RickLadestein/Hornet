using System;
using HornetEngine.Ecs;
using HornetEngine.Graphics;
using HornetEngine.Graphics.Buffers;
using HornetEngine.Util;
using System.Numerics;

namespace Sandbox
{
    class Program
    {
        public static Window w = new Window();
        public static Entity e;
        public static Camera cam;

        static void Main()
        {
            DirectoryManager.RegisterResourceDir("textures", "resources\\textures");
            DirectoryManager.RegisterResourceDir("shaders", "resources\\shaders");
            DirectoryManager.RegisterResourceDir("models", "resources\\models");
            

            w.Open("Test", 3840, 2160, WindowMode.WINDOWED);
            w.Title = "Helloworld";
            w.Redraw += W_Redraw;
            DoManualResourceAquisition();

            e = new Entity("monkeh");
            MeshComponent meshcomp = new MeshComponent();
            meshcomp.SetTargetMesh("monkey");

            MaterialComponent matcomp = new MaterialComponent();
            matcomp.SetShaderFromId("default");

            TextureComponent texcomp = new TextureComponent();
            texcomp.SetTextureUnit("laminate", HTextureUnit.Unit_0);
            texcomp.SetTextureUnit("awp_color", HTextureUnit.Unit_1);

            MonkeyScript monkeyscr = new MonkeyScript();


            e.AddComponent(meshcomp);
            e.AddComponent(matcomp);
            e.AddScript(monkeyscr);

            cam = new Camera();
            cam.ViewSettings = new CameraViewSettings()
            {
                Lens_height = 1080,
                Lens_width = 1920,
                Fov = OpenTK.Mathematics.MathHelper.DegreesToRadians(45),
                clip_min = 0.001f,
                clip_max = 1000.0f
            };
            cam.UpdateProjectionMatrix();
            cam.UpdateViewMatrix();

            e.Transform.Position += new GlmSharp.vec3(0.0f, 0.0f, 5.0f);
            w.Run();
        }


        

        private static void W_Redraw()
        {
            MaterialComponent matcomp = e.GetComponent<MaterialComponent>();
            MeshComponent meshcomp = e.GetComponent<MeshComponent>();
            TextureComponent texcomp = e.GetComponent<TextureComponent>();

            if(meshcomp.Mesh == null || matcomp.Shader == null)
            {
                Console.WriteLine("Mesh or Shader was null");
            }

            if(texcomp != null)
            {
                texcomp.Textures.Bind();
                for(int i = 0; i < texcomp.Textures.textures.Length; i++)
                {
                    if(texcomp.Textures.textures[i] != null)
                    {
                        matcomp.Shader.SetUniform($"texture_{i}", i);
                    }
                }
            }

            VertexBuffer vbuf = meshcomp.Mesh.VertexBuffer;
            vbuf.Bind();

            ShaderProgram sh = matcomp.Shader;
            sh.Bind();
            sh.SetUniform("model", e.Transform.ModelMat);
            sh.SetUniform("projection", cam.ProjectionMatrix);
            sh.SetUniform("view", cam.ViewMatrix);
            sh.SetUniform("camera_position", cam.Position);
            sh.SetUniform("camera_target", cam.Target);
            sh.SetUniform("time", 10);

            NativeWindow.GL.DrawArrays(Silk.NET.OpenGL.GLEnum.Triangles, 0, vbuf.VertexCount);
            vbuf.Unbind();
            ShaderProgram.UnbindAll();

            if (texcomp != null)
            {
                texcomp.Textures.Unbind();
            }

            foreach (MonoScript ms in e.Scripts)
            {
                ms.Update();
            }
            return;
            
        }

        #region MANUAL_RESOURCE
        private static void DoManualResourceAquisition()
        {
            Mesh m = Mesh.ImportMesh("monkey", "models", "ape.obj");
            if (m != null)
            {
                MeshResourceManager.GetInstance().AddResource("monkey", m);
            }
            else
            {
                throw new Exception("Mesh loading failed");
            }

            VertexShader vsh = new VertexShader("shaders", "multitex.vert");
            FragmentShader fsh = new FragmentShader("shaders", "multitex.frag");
            ShaderProgram prog = new ShaderProgram(vsh, fsh);
            ShaderResourceManager.GetInstance().AddResource("default", prog);

            Texture tex = new Texture("textures", "laminate1.png", false);
            TextureResourceManager.GetInstance().AddResource("laminate", tex);

            Texture tex2 = new Texture("textures", "sponza_column_c_ddn.tga", false);
            TextureResourceManager.GetInstance().AddResource("awp_color", tex2);
        }
        #endregion
    }
}
