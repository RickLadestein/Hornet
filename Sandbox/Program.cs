using HornetEngine;
using System;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        { 
            SoundManager manager = SoundManager.Instance;

            manager.addSample(1, "C:\\Users\\cools\\Documents\\resources\\cheer.ogg", 0.3f, 1.0f);
            manager.addSample(2, "C:\\Users\\cools\\Documents\\resources\\zap.ogg", 0.3f, 1.0f);

            manager.playSound(1);
            manager.playSound(2);
        }
    }
}
