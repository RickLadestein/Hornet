using System;
using HornetEngine.Graphics;
namespace Sandbox
{
    class Program
    {
        static Window w = new Window();
        static void Main(string[] args)
        {
            
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
