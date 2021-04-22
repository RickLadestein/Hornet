using HornetEngine;
using System;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        { 
            SoundManager manager = SoundManager.Instance;

            float volume = 1.0f;
            float pitch = 1.0f;

            manager.addSample(1, "cheer.ogg", 0.3f, 1.0f);
            manager.addSample(2, "cheer.ogg", 1.0f, 3.0f);

            manager.playSound(1);
            manager.playSound(2);
        }
    }
}
