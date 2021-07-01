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

            NativeWindow.GL.Disable(Silk.NET.OpenGL.EnableCap.CullFace);

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
            SoundResourceManager.Instance.ImportResource("bonk", "samples", "menu.wav");
            SoundResourceManager.Instance.ImportResource("drum", "samples", "drum_kick.wav");
            SoundResourceManager.Instance.ImportResource("guitar", "samples", "guitar_acoustic.wav");
            SoundResourceManager.Instance.ImportResource("violin", "samples", "violin_c4.wav");

            Config config = Config.Instance;

            TextureResourceManager.Instance.ImportResource("default", "textures", "laminate1.png");
            TextureResourceManager.Instance.ImportResource("drum", "textures", "drum.png");
            TextureResourceManager.Instance.ImportResource("guitar", "textures", "guitar.png");
            TextureResourceManager.Instance.ImportResource("violin", "textures", "violin.png");
            TextureResourceManager.Instance.ImportResource("qmark", "textures", "qmark.png");
        }
        #endregion
    }
}