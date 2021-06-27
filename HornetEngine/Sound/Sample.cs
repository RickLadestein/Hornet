using HornetEngine.Util;
using OpenTK.Audio.OpenAL;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace HornetEngine.Sound
{
    /// <summary>
    /// A Sample class, which will be used to hold the data of Sound Samples.
    /// </summary>
    public class Sample
    {
        public int Handle { get; private set; }

        /// <summary>
        /// The constructor of the Sound Sample
        /// </summary>
        /// <param name="givenFileLocation">A string which contains the path to the file which should be played.</param>
        public Sample(string givenFileLocation) 
        {
            InitBuffer(givenFileLocation);
        }

        public Sample(string folder_id, string file)
        {
            if(!file.EndsWith(".wav"))
            {
                String[] tokens = file.Split(".");
                throw new NotSupportedException($".{tokens[1]} extension type not supported: please only use .wav files");
            }
            string folder_dir = DirectoryManager.GetResourceDir(folder_id);
            string path = DirectoryManager.ConcatDirFile(folder_dir, file);
            InitBuffer(path);
        }

        /// <summary>
        /// A function which will be called when creating a Sample
        /// 
        /// This function will initialize the buffers for the specific sample.
        /// </summary>
        private void InitBuffer(string givenFileLocation)
        {
            // Initialize the buffer
            Handle = AL.GenBuffer();

            // Prints the file location to the console
            Console.WriteLine("Sample {0} initialized", givenFileLocation);

            //int channels, bits_per_sample, sample_rate;
            //byte[] sound_data = loadWave(File.Open(givenFileLocation, FileMode.Open), out channels, out bits_per_sample, out sample_rate);

            Stream fstream = File.OpenRead(givenFileLocation);
            LoadWave(fstream, out chunk_descriptor dsc, out fmt_subchunk fmt, out data_chunk dta);

            // Create an IntPtr which points towards the sound_data
            GCHandle pinnedArray = GCHandle.Alloc(dta.data, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            ALFormat al_format = GetSoundFormat(fmt.num_channels, fmt.bits_per_sample);
            AL.BufferData(Handle, al_format, pointer, dta.data.Length, fmt.sample_rate);

            // Free the array to prevent memory leaks
            pinnedArray.Free();
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

                //int format_chunk_size = reader.ReadInt32();
                int fmt_chunk_size = BinaryPrimitives.ReadInt32LittleEndian(reader.ReadBytes(4));
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(6));
                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;
                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        private void LoadWave(Stream stream, out chunk_descriptor primary_header, out fmt_subchunk format_header, out data_chunk data_chunk)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                primary_header = chunk_descriptor.Parse(reader);
                format_header = fmt_subchunk.Parse(reader);
                data_chunk = data_chunk.Parse(reader);
            }
        }

        /// <summary>
        /// A function which will get the sound format.
        /// </summary>
        /// <param name="channels">The amount of channels within the sound file.</param>
        /// <param name="bits">The sample frequency in Hz.</param>
        /// <returns></returns>
        private ALFormat GetSoundFormat(int channels, int bits)
        {
            if(bits > 16)
            {
                throw new NotSupportedException("32 bit audio channels not supported");
            }

            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }
    }

    public struct chunk_descriptor
    {
        public string signature;
        public int chunk_size;
        public string format;

        public static chunk_descriptor Parse(BinaryReader reader)
        {
            chunk_descriptor output = new chunk_descriptor
            {
                signature = new string(reader.ReadChars(4)),
                chunk_size = reader.ReadInt32(),
                format = new string(reader.ReadChars(4))
            };
            return output;
        }
    }

    public struct fmt_subchunk
    {
        public string chunk_id;
        public int sub_chunk_size;
        public short audio_format;
        public short num_channels;
        public int sample_rate;
        public int byte_rate;
        public short block_align;
        public short bits_per_sample;

        public static fmt_subchunk Parse(BinaryReader reader)
        {
            fmt_subchunk output = new fmt_subchunk
            {
                chunk_id = new string(reader.ReadChars(4)),
                sub_chunk_size = reader.ReadInt32(),
                audio_format = reader.ReadInt16(),
                num_channels = reader.ReadInt16(),
                sample_rate = reader.ReadInt32(),
                byte_rate = reader.ReadInt32(),
                block_align = reader.ReadInt16(),
                bits_per_sample = reader.ReadInt16()
            };
            return output;
        }
    }

    public struct data_chunk
    {
        public string chunk_id;
        public int chunk_size;
        public byte[] data;

        public static data_chunk Parse(BinaryReader reader)
        {

            //Read until data block is reached
            string data_id = "";
            while(!data_id.Equals("data"))
            {
                data_id = new string(reader.ReadChars(4));
            }

            data_chunk output = new data_chunk
            {
                chunk_id = data_id,
                chunk_size = reader.ReadInt32(),
                data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position))
            };
            return output;
        }
    }
}