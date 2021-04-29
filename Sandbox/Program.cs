using HornetEngine;
using System;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        { 
            SoundManager manager = SoundManager.Instance;

            manager.addSample(1, "C:\\Users\\cools\\Documents\\resources\\menu.wav", 1.0f, 1.0f);
            manager.playSound(1);
        }
    }
}
