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

        static void Main()
        {
            DirectoryManager.RegisterResourceDir("textures", "resources\\textures");
            DirectoryManager.RegisterResourceDir("shaders", "resources\\shaders");
            DirectoryManager.RegisterResourceDir("models", "resources\\models");
            DirectoryManager.RegisterResourceDir("samples", "resources\\samples");

            SoundManager mgr = SoundManager.Instance;

            w.Open("Test", 1920, 1080, WindowMode.WINDOWED);
            w.Title = "Helloworld";

            DoManualResourceAquisition();

            player = new Entity("Player");
            PlayerScript pscr = new PlayerScript
            {
                mouse = w.Mouse,
                keyboard = w.Keyboard
            };
            player.AddScript(pscr);

            player.AddComponent(new AudioListenerComponent());
            Scene.Instance.AddEntity(player);


            Entity henk = new Entity("Henk");
            HenkScript hs = new HenkScript();
            hs.cam = Camera.Primary;
            henk.AddScript(hs);

            SoundSourceComponent ssc = new SoundSourceComponent();
            henk.AddComponent(ssc);

            Mesh mesh = Mesh.ImportMesh("henkmesh", "models", "ape.obj");
            MeshResourceManager.Instance.AddResource("henkmesh" , mesh);
            MeshComponent ms = new MeshComponent();
            ms.SetTargetMesh("henkmesh");
            henk.AddComponent(ms);

            InterfaceRenderComponent mrc = new InterfaceRenderComponent();
            henk.AddComponent(mrc);


            MaterialComponent matcomp = new MaterialComponent();
            matcomp.SetShaderFromId("default");
            matcomp.SetTextureUnit("laminate", HTextureUnit.Unit_0);
            henk.AddComponent(matcomp);

            henk.Transform.Position = new GlmSharp.vec3(0.0f, 0.0f, 5.0f);
            Scene.Instance.AddEntity(henk);







            w.Run();
        }

        #region MANUAL_RESOURCE
        private static void DoManualResourceAquisition()
        {
            VertexShader dvsh = new VertexShader("shaders", "line_default.vert");
            FragmentShader dfsh = new FragmentShader("shaders", "line_default.frag");
            ShaderProgram dprog = new ShaderProgram(dvsh, dfsh);
            ShaderResourceManager.Instance.AddResource("default_line", dprog);

            VertexShader vsh = new VertexShader("shaders", "default.vert");
            FragmentShader fsh = new FragmentShader("shaders", "default.frag");
            ShaderProgram prog = new ShaderProgram(vsh, fsh);
            ShaderResourceManager.Instance.AddResource("default", prog);

            ShaderProgram prog1 = new ShaderProgram(new VertexShader("shaders", "deferred_pre.vert"), new FragmentShader("shaders", "deferred_pre.frag"));
            ShaderResourceManager.Instance.AddResource("deferred_pre", prog1);



            Texture tex = new Texture("textures", "laminate1.png", false);
            TextureResourceManager.Instance.AddResource("laminate", tex);

            Texture tex2 = new Texture("textures", "sponza_column_c_ddn.tga", false);
            TextureResourceManager.Instance.AddResource("default", tex2);

            Sample samp = new Sample("samples", "menu.wav");
            SoundManager.Instance.AddResource("bonk", samp);

            //Scene.Instance.LoadScene("models", "sponza.obj");
        }
        #endregion
    }
}