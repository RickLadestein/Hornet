using OpenTK.Audio.OpenAL;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace HornetEngine
{
    /// <summary>
    /// A Sample class, which will be used to hold the data of Sound Samples.
    /// </summary>
    public class Sample
    {
        private string givenFileLocation;
        private int buffer;

        /// <summary>
        /// The constructor of the Sound Sample
        /// </summary>
        /// <param name="givenFileLocation">A string which contains the path to the file which should be played.</param>
        public Sample(string givenFileLocation) 
        {
            this.givenFileLocation = givenFileLocation;

            initBuffer();
        }

        /// <summary>
        /// A function which will be called when creating a Sample
        /// 
        /// This function will initialize the buffers for the specific sample.
        /// </summary>
        public void initBuffer()
        {
            // Initialize the buffer
            buffer = AL.GenBuffer();

            // Prints the file location to the console
            Console.WriteLine("Sample {0} initialized", givenFileLocation);

            int channels, bits_per_sample, sample_rate;
            byte[] sound_data = loadWave(File.Open(givenFileLocation, FileMode.Open), out channels, out bits_per_sample, out sample_rate);

            // Create an IntPtr which points towards the sound_data
            GCHandle pinnedArray = GCHandle.Alloc(sound_data, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            // Assign the sound to the buffer
            AL.BufferData(buffer, getSoundFormat(channels, bits_per_sample), pointer, sound_data.Length, sample_rate);

            // Free the array to prevent memory leaks
            pinnedArray.Free();
        }

        /// <summary>
        /// A function which will play a sample's sound.
        /// </summary>
        /// <param name="src">The SoundSource on which the sound should be played.</param>
        public void playSound(SoundSource src)
        {
            // Initialize the source and the state
            int source = src.getSource();
            int state;

            // Initialize and play the sound
            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.SourcePlay(source);

            // Check when the sound ends
            do
            {
                Thread.Sleep(250);
                AL.GetSource(source, ALGetSourcei.SourceState, out state);
            }
            while ((ALSourceState)state == ALSourceState.Playing);

            // Stop the sound and delete the source / buffer
            AL.SourceStop(source);
        }

        /// <summary>
        /// A function which will load the sample's file
        /// </summary>
        /// <param name="stream">The stream which will be used to open the file</param>
        /// <param name="channels">The amount of channels within the sound.</param>
        /// <param name="bits">Sound encoding level</param>
        /// <param name="rate">The sample frequency in Hz.</param>
        /// <returns></returns>
        private byte[] loadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave (.wav) file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave (.wav) file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        /// <summary>
        /// A function which will get the sound format.
        /// </summary>
        /// <param name="channels">The amount of channels within the sound file.</param>
        /// <param name="bits">The sample frequency in Hz.</param>
        /// <returns></returns>
        private ALFormat getSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }
    }
}