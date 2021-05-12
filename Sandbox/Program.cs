using System;
using HornetEngine.Graphics;
using HornetEngine.Util;
namespace Sandbox
{
    class Program
    {
        static Window w = new Window();
        static void Main()
        {
            DirectoryManager.RegisterResourceDir("textures", "resources\\textures");
            DirectoryManager.RegisterResourceDir("shaders", "resources\\shaders");
            DirectoryManager.RegisterResourceDir("models", "resources\\models");

            w.Open("Test", 1080, 720, false);
            w.Title = "Helloworld";
            w.Redraw += W_Redraw;
            w.Move += W_Move;
            w.Run();
        }

        private static void W_Move(System.Numerics.Vector2 newpos)
        {
            w.SetTitle($"Position: {newpos}");
        }

        private static void W_Redraw(float timestep)
        {
            return;
            
        }
    }
}
