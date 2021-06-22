using System;
using HornetEngine.Ecs;
using HornetEngine.Graphics;
using HornetEngine.Graphics.Buffers;
using HornetEngine.Util;
using System.Numerics;
using HornetEngine;
using System.Threading.Tasks;
using HornetEngine.Input;
using HornetEngine.Sound;

namespace Sandbox
{
    class Program
    {
        public static Window w = new Window();
        public static Entity line_entity;
        public static Entity player;
        public static Scene sc;

        static void Main()
        {
            DirectoryManager.RegisterResourceDir("textures", "resources\\textures");
            DirectoryManager.RegisterResourceDir("shaders", "resources\\shaders");
            DirectoryManager.RegisterResourceDir("models", "resources\\models");
            DirectoryManager.RegisterResourceDir("samples", "resources\\samples");

            SoundManager mgr = SoundManager.Instance;

            w.Open("Test", 1920, 1080, WindowMode.WINDOWED_FULLSCREEN);
            w.Title = "Helloworld";
            w.Redraw += W_Redraw;
            
            sc = new Scene();
            DoManualResourceAquisition();

            player = new Entity("Player");
            PlayerScript pscr = new PlayerScript
            {
                mouse = w.Mouse,
                keyboard = w.Keyboard,
                scene = sc
            };
            player.AddScript(pscr);

            player.AddComponent(new AudioListenerComponent());
            sc.scene_content.Add(player);

            line_entity = new Entity("line");
            LineRenderComponent rc = new LineRenderComponent();
            rc.AddLine(new GlmSharp.vec3(0, 0, 5), new GlmSharp.vec3(5, 0, 5));
            rc.AddLine(new GlmSharp.vec3(5, 0, 5), new GlmSharp.vec3(0, 2, 5));
            rc.Base_Color = new GlmSharp.vec4(1.0f, 0.0f, 0.0f, 1.0f);
            rc.Line_width = 100.0f;
            rc.BuildBuffer();
            line_entity.AddComponent(rc);

            line_entity.AddComponent(new SoundSourceComponent());

            sc.scene_content.Add(line_entity);

            NativeWindow.GL.PolygonMode(Silk.NET.OpenGL.MaterialFace.FrontAndBack, Silk.NET.OpenGL.PolygonMode.Line);
            w.Run();
        }

        private static void W_Redraw()
        {
            foreach (Entity en in sc.scene_content)
            {
                if(en.HasComponent<MeshComponent>())
                {
                    Renderer.Instance.RenderEntity(Camera.Primary, en);
                }
            }

            foreach (Entity en in sc.scene_content)
            {
                LineRenderComponent linercomp = en.GetComponent<LineRenderComponent>();
                if (linercomp != null)
                {
                    linercomp.Render(Camera.Primary);
                }
            }

            sc.UpdateScene();
            return;
        }

        #region MANUAL_RESOURCE
        private static void DoManualResourceAquisition()
        {
            VertexShader dvsh = new VertexShader("shaders", "line_default.vert");
            FragmentShader dfsh = new FragmentShader("shaders", "line_default.frag");
            ShaderProgram dprog = new ShaderProgram(dvsh, dfsh);
            ShaderResourceManager.GetInstance().AddResource("default_line", dprog);

            VertexShader vsh = new VertexShader("shaders", "block.vert");
            FragmentShader fsh = new FragmentShader("shaders", "block.frag");
            ShaderProgram prog = new ShaderProgram(vsh, fsh);
            ShaderResourceManager.GetInstance().AddResource("default", prog);

            Texture tex = new Texture("textures", "laminate1.png", false);
            TextureResourceManager.GetInstance().AddResource("default", tex);

            Texture tex2 = new Texture("textures", "sponza_column_c_ddn.tga", false);
            TextureResourceManager.GetInstance().AddResource("awp_color", tex2);

            Sample samp = new Sample("samples", "laser.wav");
            SoundManager.Instance.AddResource("bonk", samp);

            sc.LoadScene("models", "sponza.obj");
        }
        #endregion
    }
}