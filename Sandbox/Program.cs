using System;
using HornetEngine.Ecs;
using HornetEngine.Graphics;
using HornetEngine.Graphics.Buffers;
using HornetEngine.Util;
using HornetEngine.Input.Touch_Recognition;
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
            setupGraphicsDemo();
        }

        private static void setupGraphicsDemo()
        {
            DirectoryManager.RegisterResourceDir("textures", "resources\\textures");
            DirectoryManager.RegisterResourceDir("shaders", "resources\\shaders");
            DirectoryManager.RegisterResourceDir("models", "resources\\models");
            DirectoryManager.RegisterResourceDir("samples", "resources\\samples");

            SoundResourceManager mgr = SoundResourceManager.Instance;

            w.Open("Test", 1920, 1080, WindowMode.WINDOWED);
            w.Title = "Helloworld";

            DoManualResourceAquisition();

            //player = new Entity("Player");
            //PlayerScript pscr = new PlayerScript
            //{
            //    mouse = w.Mouse,
            //    keyboard = w.Keyboard
            //};
            //player.AddScript(pscr);

            //player.AddComponent(new AudioListenerComponent());
            //Scene.Instance.AddEntity(player);


            Entity henk = new Entity("plane");
            HenkScript hs = new HenkScript();
            henk.AddScript(hs);

            SoundSourceComponent ssc = new SoundSourceComponent();
            henk.AddComponent(ssc);

            Mesh mesh = Mesh.ImportMesh("planemesh", "models", "plane.obj");
            MeshResourceManager.Instance.AddResource("planemesh" , mesh);
            MeshComponent ms = new MeshComponent();
            ms.SetTargetMesh("planemesh");
            henk.AddComponent(ms);

            InterfaceRenderComponent mrc = new InterfaceRenderComponent();
            henk.AddComponent(mrc);

            MaterialComponent matcomp = new MaterialComponent();
            matcomp.SetShaderFromId("default");
            //matcomp.SetTextureUnit("drum", HTextureUnit.Unit_0);
            henk.AddComponent(matcomp);

            henk.Transform.Position = new GlmSharp.vec3(0.0f, 0.0f, 20.0f);
            henk.Transform.Scale = new GlmSharp.vec3(5.0f, 5.0f, 5.0f);
            henk.Transform.Rotate(new GlmSharp.vec3(0.0f, 0.0f, 1.0f), 90);
            Scene.Instance.AddEntity(henk);

            w.Run();
        }

        #region MANUAL_RESOURCE
        private static void DoManualResourceAquisition()
        {
            

            TextureResourceManager.Instance.ImportResource("default", "textures", "laminate1.png");

            SoundResourceManager.Instance.ImportResource("bonk", "samples", "menu.wav");

            Config config = Config.Instance;
            SoundManager manager = SoundManager.Instance;
            Texture tex = new Texture("textures", "laminate1.png", false);
            TextureResourceManager.Instance.AddResource("default", tex);

            Texture tex_drum = new Texture("textures", "drum.png", false);
            TextureResourceManager.Instance.AddResource("drum", tex_drum);

            Texture tex_guitar = new Texture("textures", "guitar.png", false);
            TextureResourceManager.Instance.AddResource("guitar", tex_guitar);

            Texture tex_violin= new Texture("textures", "violin.png", false);
            TextureResourceManager.Instance.AddResource("violin", tex_violin);

            Sample samp = new Sample("samples", "menu.wav");
            SoundManager.Instance.AddResource("bonk", samp);

            //Scene.Instance.LoadScene("models", "sponza.obj");
        }
        #endregion
    }
}