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
        public static Entity line_entity;
        public static Camera cam;
        public static Scene sc;

        static void Main()
        {
            DirectoryManager.RegisterResourceDir("textures", "resources\\textures");
            DirectoryManager.RegisterResourceDir("shaders", "resources\\shaders");
            DirectoryManager.RegisterResourceDir("models", "resources\\models");
            

            w.Open("Test", 1920, 1080, WindowMode.WINDOWED);
            w.Title = "Helloworld";
            w.Redraw += W_Redraw;
            
            sc = new Scene();
            DoManualResourceAquisition();

            line_entity = new Entity("line");

            LineRenderComponent rc = new LineRenderComponent();
            rc.AddLine(new GlmSharp.vec3(0, 0, 5), new GlmSharp.vec3(5, 0, 5));
            rc.AddLine(new GlmSharp.vec3(5, 0, 5), new GlmSharp.vec3(0, 2, 5));
            rc.Base_Color = new GlmSharp.vec4(1.0f, 0.0f, 0.0f, 1.0f);
            rc.Line_width = 100.0f;
            rc.BuildBuffer();
            line_entity.AddComponent(rc);

            sc.scene_content.Add(line_entity);

            NativeWindow.GL.PolygonMode(Silk.NET.OpenGL.MaterialFace.FrontAndBack, Silk.NET.OpenGL.PolygonMode.Line);

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

            w.Run();
        }

        private static void W_Redraw()
        {
            foreach(Entity en in sc.scene_content)
            {
                RenderComponent rendercomp = en.GetComponent<RenderComponent>();
                if(rendercomp != null)
                {
                    rendercomp.Render(cam);
                }
            }

            foreach (Entity en in sc.scene_content)
            {
                LineRenderComponent linercomp = en.GetComponent<LineRenderComponent>();
                if (linercomp != null)
                {
                    linercomp.Render(cam);
                }
            }
            return;
            
        }

        #region MANUAL_RESOURCE
        private static void DoManualResourceAquisition()
        {
            //Mesh m = Mesh.ImportMesh("monkey", "models", "ape.obj");
            //if (m != null)
            //{
            //    MeshResourceManager.GetInstance().AddResource("monkey", m);
            //}
            //else
            //{
            //    throw new Exception("Mesh loading failed");
            //}

            VertexShader dvsh = new VertexShader("shaders", "line_default.vert");
            FragmentShader dfsh = new FragmentShader("shaders", "line_default.frag");
            ShaderProgram dprog = new ShaderProgram(dvsh, dfsh);
            ShaderResourceManager.GetInstance().AddResource("default_line", dprog);

            VertexShader vsh = new VertexShader("shaders", "block.vert");
            FragmentShader fsh = new FragmentShader("shaders", "block.frag");
            ShaderProgram prog = new ShaderProgram(vsh, fsh);
            ShaderResourceManager.GetInstance().AddResource("default", prog);

            Texture tex = new Texture("textures", "laminate1.png", false);
            TextureResourceManager.GetInstance().AddResource("laminate", tex);

            Texture tex2 = new Texture("textures", "sponza_column_c_ddn.tga", false);
            TextureResourceManager.GetInstance().AddResource("awp_color", tex2);

            //sc.LoadScene("models", "sponza.obj");
        }
        #endregion
    }
}
