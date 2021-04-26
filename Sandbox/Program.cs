using System;
using HornetEngine.Graphics;
namespace Sandbox
{
    class Program
    {
        static Window w = new Window();
        static int counter = 0;
        static void Main(string[] args)
        {
            
            w.Open("Test", 1080, 720, false);
            w.Title = "Helloworld";
            w.Redraw += W_Redraw;
            w.Run();
            Console.WriteLine("Hello World!");
        }

        private static void W_Redraw(float timestep)
        {
            counter += 1;
            if(counter > 1000)
            {
                counter = 0;
                if (timestep != 0.0f)
                {
                    float fps = 1 / timestep;
                    w.Title = $"Fps: {fps.ToString("n1")}";
                }
            }
            
        }
    }
}
