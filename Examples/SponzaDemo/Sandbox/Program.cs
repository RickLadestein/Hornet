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

            SoundResourceManager mgr = SoundResourceManager.Instance;

            w.Open("Sponza", 1920, 1080, WindowMode.WINDOWED);
            NativeWindow.GL.Disable(Silk.NET.OpenGL.GLEnum.Blend);
            TextureResourceManager.Instance.ImportResource("default", "textures", "laminate1.png");
            Scene.Instance.LoadScene("models", "sponza.obj");

            player = new Entity("Player");
            PlayerScript pscr = new PlayerScript
            {
                mouse = w.Mouse,
                keyboard = w.Keyboard
            };
            player.AddScript(pscr);

            player.AddComponent(new AudioListenerComponent());
            Scene.Instance.AddEntity(player);

            
            w.Run();
        }
    }
}