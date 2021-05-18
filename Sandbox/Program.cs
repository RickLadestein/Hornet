using HornetEngine;
using System;
using System.Numerics;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            SoundManager manager = SoundManager.Instance;
            Entity e = new Entity(false);

            manager.addSample(1, "C:\\Users\\cools\\Documents\\resources\\tone.wav");

            e.setVolume(0.1f);
            manager.getSample(1).playSound(e);

            manager.getListener().setPosition(new Vector3(20.0f, 0.0f, 0.0f));
            manager.getSample(1).playSound(e);
            manager.getListener().setPosition(new Vector3(0.0f, 0.0f, 20.0f));
            manager.getSample(1).playSound(e);
        }
    }
}