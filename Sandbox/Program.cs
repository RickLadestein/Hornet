﻿using HornetEngine;
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
            manager.playSound(1, e);

            manager.setPos(new Vector3(20.0f, 0.0f, 0.0f));
            manager.playSound(1, e);
        }
    }
}