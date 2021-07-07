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
        public static Entity player;

        static void Main()
        {
            DirectoryManager.RegisterResourceDir("textures", "resources\\textures");
            DirectoryManager.RegisterResourceDir("shaders", "resources\\shaders");
            DirectoryManager.RegisterResourceDir("models", "resources\\models");
            DirectoryManager.RegisterResourceDir("samples", "resources\\samples");

            w.Open("Example", 1920, 1080, WindowMode.WINDOWED);

            TextureResourceManager.Instance.ImportResource("default", "textures", "laminate1.png");
            TextureResourceManager.Instance.ImportResource("laminate", "textures", "laminate2.png");

            MeshResourceManager.Instance.ImportResource("squirel", "models", "squirel.obj");
            MeshResourceManager.Instance.ImportResource("cat", "models", "cat.obj");
            MeshResourceManager.Instance.ImportResource("dog", "models", "dog.obj");
            MeshResourceManager.Instance.ImportResource("surface", "models", "plane.obj");

            ShaderResourceManager.Instance.ImportResource("example_shader", "shaders", "example.vert", "example.frag");

            SoundResourceManager.Instance.ImportResource("cat", "samples", "cat_1.wav");
            SoundResourceManager.Instance.ImportResource("dog", "samples", "dog_1.wav");

            player = new Entity("Player");
            PlayerScript pscr = new PlayerScript();
            player.AddScript(pscr);

            //Setup the entity
            Entity en = new Entity("Actor");
            EntityScript escr = new EntityScript();
            MaterialComponent matcomp = new MaterialComponent();
            MeshComponent meshcomp = new MeshComponent();
            InterfaceRenderComponent rcomp = new InterfaceRenderComponent();
            SoundSourceComponent scomp = new SoundSourceComponent();
            matcomp.SetShaderFromId("example_shader");
            matcomp.SetTextureUnit("default", HTextureUnit.Unit_0);
            meshcomp.SetTargetMesh("surface");

            escr.meshcomp = meshcomp;
            escr.matcomp = matcomp;
            escr.soundsourcecomp = scomp;
            
            en.AddScript(escr);
            en.AddComponent(matcomp);
            en.AddComponent(meshcomp);
            en.AddComponent(rcomp);
            en.AddComponent(scomp);


            player.AddComponent(new AudioListenerComponent());
            Scene.Instance.AddEntity(player);
            Scene.Instance.AddEntity(en);
            
            w.Run();
        }
    }
}