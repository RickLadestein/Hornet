using System;
using HornetEngine.Graphics;
namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Window w = new Window();
            w.Open("Test", 1080, 720, false);
            w.Run();
            Console.WriteLine("Hello World!");
        }
    }
}
