using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace HornetEngine
{
    /// <summary>
    /// The SoundManager class can be used to manage the sounds within the application.
    /// </summary>
    public class SoundManager
    {
        private static SoundManager instance = null;
        private static readonly object padlock = new object();

        private Dictionary<int, Sample> samples;
        private Listener listener;

        /// <summary>
        /// The constructor of the SoundManager
        /// </summary>
        unsafe SoundManager() 
        {
            var device = ALC.OpenDevice(null);
            var context = ALC.CreateContext(device, (int*)null);
            ALC.MakeContextCurrent(context);

            samples = new Dictionary<int, Sample>();
            listener = Listener.Instance;
        }

        /// <summary>
        /// A function which will allow the user to add a new sample to the manager.
        /// </summary>
        /// <param name="id">The ID of the new sample.</param>
        /// <param name="filename">The name of the file for the new sample, for ex. cheers.ogg</param>
        public void addSample(int id, string filename)
        {
            // Initialize the new sample, based on the given values
            Sample newSample = new Sample(filename);

            try
            {
                this.samples.Add(id, newSample);
            } catch (ArgumentException)
            {
                Console.WriteLine("An element with this ID already exists.");
            }
        }

        /// <summary>
        /// A function which will remove a sample from the sample manager.
        /// </summary>
        /// <param name="id">The ID of the sample which should be removed.</param>
        /// <returns></returns>
        public bool removeSample(int id)
        {
            try
            {
                samples.Remove(id);
                return true;
            } catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///  A function which will obtain a Sample.
        /// </summary>
        /// <param name="givenId">The id of the sample which should be returned.</param>
        /// <returns>The sample which contains the given ID.</returns>
        public Sample getSample(int givenId)
        {
            try
            {
                // Attempt to get the sample within the SoundManager and return it
                Sample toReturn = samples[givenId];
                return toReturn;
            } catch (Exception)
            {
                // Return null if the sample does not exist
                Console.WriteLine("This sample does not exist.");
                return null;
            }
        }

        /// <summary>
        /// A function which will return the user's Listener
        /// </summary>
        /// <returns>The user's Listener.</returns>
        public Listener getListener()
        {
            return this.listener;
        }

        /// <summary>
        /// A method to get the instance of the SoundManager.
        /// The lock ensures that the singleton is thread-safe.
        /// </summary>
        public static SoundManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if(instance == null)
                    {
                        instance = new SoundManager();
                    }
                    return instance;
                }
            }
        }
    }
}
