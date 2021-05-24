using System;
using HornetEngine.Ecs;
using HornetEngine.Graphics;
using HornetEngine.Graphics.Buffers;
using HornetEngine.Util;
using Silk.NET.OpenGL;
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

            w.Open("Test", 1080, 720, false);
            w.Title = "Helloworld";
            w.Redraw += W_Redraw;

            NativeWindow.GL.Enable(EnableCap.DepthTest);
            NativeWindow.GL.DepthFunc(DepthFunction.Less);



            e = new Entity("monkeh");
            MeshComponent meshcomp = new MeshComponent();
            Mesh m = Mesh.ImportMesh("monkey", "models", "ape.obj");
            if(m != null)
            {
                MeshResourceManager.GetInstance().AddResource("monkey", m);
            } else
            {
                throw new Exception("Mesh loading failed");
            }
            meshcomp.SetTargetMesh("monkey");

            MaterialComponent matcomp = new MaterialComponent();
            VertexShader vsh = new VertexShader("shaders", "toon.vert");
            FragmentShader fsh = new FragmentShader("shaders", "toon.frag");
            ShaderProgram prog = new ShaderProgram(vsh, fsh);
            ShaderResourceManager.GetInstance().AddResource("default", prog);
            matcomp.SetShaderFromId("default");

            e.AddComponent(meshcomp);
            e.AddComponent(matcomp);

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

            e.Transform.Position += new System.Numerics.Vector3(0.0f, 0.0f, 5.0f);
            w.Run();
        }


        

        private static void W_Redraw(float timestep)
        {
            MaterialComponent matcomp = e.GetComponent<MaterialComponent>();
            MeshComponent meshcomp = e.GetComponent<MeshComponent>();

            if(meshcomp.Mesh == null || matcomp.Shader == null)
            {
                Console.WriteLine("Mesh or Shader was null");
            }

            VertexBuffer vbuf = meshcomp.Mesh.VertexBuffer;
            vbuf.Bind();

            ShaderProgram sh = matcomp.Shader;
            sh.Bind();
            sh.SetUniform("model", e.Transform.ModelMat);
            sh.SetUniform("projection", cam.ProjectionMatrix);
            sh.SetUniform("view", cam.ViewMatrix);

            NativeWindow.GL.DrawArrays(Silk.NET.OpenGL.GLEnum.Triangles, 0, vbuf.VertexCount);
            vbuf.Unbind();
            ShaderProgram.UnbindAll();

            Vector3 rot_movement = new Vector3(0.0f, 10.0f, 0.0f) * timestep;
            e.Transform.Rotation = e.Transform.Rotation + rot_movement;

            return;
            
        }
    }
}
