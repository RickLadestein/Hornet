using HornetEngine;
using System;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            SoundManager manager = SoundManager.Instance;
            Entity e = new Entity();

            manager.addSample(1, "C:\\Users\\cools\\Documents\\resources\\menu.wav");
            manager.addSample(2, "C:\\Users\\cools\\Documents\\resources\\cheer.ogg");


            manager.playSound(1, e);

            e.setPitch(22.0f);
            manager.playSound(1, e);

            e.setVolume(0.3f);
            manager.playSound(1, e);
        }
    }
}