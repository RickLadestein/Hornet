using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;

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

        /// <summary>
        /// The constructor of the SoundManager
        /// </summary>
        unsafe SoundManager() 
        {
            var device = ALC.OpenDevice(null);
            var context = ALC.CreateContext(device, (int*)null);
            ALC.MakeContextCurrent(context);
            samples = new Dictionary<int, Sample>();
        }

        /// <summary>
        /// A function which will allow the user to add a new sample to the manager.
        /// </summary>
        /// <param name="id">The ID of the new sample.</param>
        /// <param name="filename">The name of the file for the new sample, for ex. cheers.ogg</param>
        /// <param name="volume">The volume which should be used to play the sample.</param>
        public void addSample(int id, string filename, float volume)
        {
            // Initialize the new sample, based on the given values
            Uri sampleUri = new Uri($"resources/{filename}", UriKind.Relative);
            Sample newSample = new Sample(sampleUri, volume);

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
        /// A function which will play the sound of a sample within the SoundManager using an ID.
        /// </summary>
        /// <param name="givenId"></param>
        public void playSound(int givenId)
        {
            // Try to play the sound of the sample, if it exists
            try
            {
                Sample tempSample = samples[givenId];
                tempSample.playSound();

            } catch (Exception)
            {
                Console.WriteLine("This sample does not exist.");
            }
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
