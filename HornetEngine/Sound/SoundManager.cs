using HornetEngine.Util;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace HornetEngine.Sound
{
    /// <summary>
    /// The SoundManager class can be used to manage the sounds within the application.
    /// </summary>
    public class SoundManager : ResourceManager<Sample>
    {
        private static SoundManager instance;
        /// <summary>
        /// A method to get the instance of the SoundManager.
        /// The lock ensures that the singleton is thread-safe.
        /// </summary>
        public static SoundManager Instance
        {
            get
            {
                    if (instance == null)
                    {
                        instance = new SoundManager();
                    }
                    return instance;
            }
        }

        /// <summary>
        /// The constructor of the SoundManager
        /// </summary>
        unsafe SoundManager() 
        {
            var device = ALC.OpenDevice(null);
            var context = ALC.CreateContext(device, (int*)null);
            ALC.MakeContextCurrent(context);
        }
    }
}
