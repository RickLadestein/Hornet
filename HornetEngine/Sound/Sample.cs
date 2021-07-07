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

        /// <summary>
        /// The constructor of the Sound Sample
        /// </summary>
        /// <param name="folder_id">The folder ID</param>
        /// <param name="file">The file name</param>
        /// <exception cref="ArgumentException">Throws an ArgumentException</exception>
        public Sample(string folder_id, string file)
        {
            if(folder_id == null || folder_id.Length == 0)
            {
                throw new ArgumentException("folder_id cannot be null or empty");
            }

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("file location cannot be null or empty");
            }


            if (!file.EndsWith(".wav"))
            {
                String[] tokens = file.Split(".");
                throw new NotSupportedException($".{tokens[1]} extension type not supported: please only use .wav files");
            }
            string folder_dir = DirectoryManager.GetResourceDir(folder_id);
            if(folder_dir == null || folder_dir.Length == 0)
            {
                throw new Exception($"Folder with id {folder_id} was not found within");
            }
            string path = DirectoryManager.ConcatDirFile(folder_dir, file);
            InitBuffer(path);
        }

        private void InitBuffer(string givenFileLocation)
        {
            // Initialize the buffer
            Handle = AL.GenBuffer();

            // Prints the file location to the console
            Console.WriteLine("Sample {0} initialized", givenFileLocation);

            //int channels, bits_per_sample, sample_rate;
            //byte[] sound_data = loadWave(File.Open(givenFileLocation, FileMode.Open), out channels, out bits_per_sample, out sample_rate);

            Stream fstream = File.OpenRead(givenFileLocation);
            LoadWave(fstream, out DescriptorChunk dsc, out FormatChunk fmt, out DataChunk dta);

            // Create an IntPtr which points towards the sound_data
            GCHandle pinnedArray = GCHandle.Alloc(dta.data, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            ALFormat al_format = GetSoundFormat(fmt.num_channels, fmt.bits_per_sample);
            AL.BufferData(Handle, al_format, pointer, dta.data.Length, fmt.sample_rate);

            // Free the array to prevent memory leaks
            pinnedArray.Free();
        }

        private void LoadWave(Stream stream, out DescriptorChunk primary_header, out FormatChunk format_header, out DataChunk data_chunk)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                primary_header = DescriptorChunk.Parse(reader);
                format_header = FormatChunk.Parse(reader);
                data_chunk = DataChunk.Parse(reader);
            }
        }

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

    /// <summary>
    /// Struct for storing Chunk Descriptor data
    /// </summary>
    public struct DescriptorChunk
    {
        /// <summary>
        /// The chunk signature string
        /// </summary>
        public string signature;

        /// <summary>
        /// The size of the Chunk Descriptor
        /// </summary>
        public int chunk_size;

        /// <summary>
        /// The format of the File
        /// </summary>
        public string format;

        /// <summary>
        /// Tries to parse a chunk descriptor from given stream
        /// </summary>
        /// <param name="reader">The binary reader to the opened audio file stream</param>
        /// <returns>Filled <c>DescriptorChunk</c></returns>
        /// <exception cref="Exception"></exception>
        public static DescriptorChunk Parse(BinaryReader reader)
        {
            DescriptorChunk output = new DescriptorChunk
            {
                signature = new string(reader.ReadChars(4)),
                chunk_size = reader.ReadInt32(),
                format = new string(reader.ReadChars(4))
            };
            return output;
        }
    }


    /// <summary>
    /// Struct for storing Format Chunk data
    /// </summary>
    public struct FormatChunk
    {
        /// <summary>
        /// The id of this format chunk
        /// </summary>
        public string chunk_id;

        /// <summary>
        /// The size of the format chunk
        /// </summary>
        public int sub_chunk_size;

        /// <summary>
        /// The format the audio is stored in (wave)
        /// </summary>
        public short audio_format;

        /// <summary>
        /// The amount of channels
        /// </summary>
        public short num_channels;

        /// <summary>
        /// The sound sample rate in Hz
        /// </summary>
        public int sample_rate;

        /// <summary>
        /// The sound byte rate Bytes/Second
        /// </summary>
        public int byte_rate;

        /// <summary>
        /// 
        /// </summary>
        public short block_align;

        /// <summary>
        /// The amount of bits per sound sample
        /// </summary>
        public short bits_per_sample;


        /// <summary>
        /// Tries to parse a format chunk from given stream
        /// </summary>
        /// <param name="reader">The binary reader to the opened audio file stream</param>
        /// <returns>Filled <c>FormatChunk</c></returns>
        /// /// <exception cref="Exception"></exception>
        public static FormatChunk Parse(BinaryReader reader)
        {
            FormatChunk output = new FormatChunk
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

    /// <summary>
    /// Struct for storing Audio data
    /// </summary>
    public struct DataChunk
    {
        /// <summary>
        /// The ID of the data stored in this chunk
        /// </summary>
        public string chunk_id;

        /// <summary>
        /// The total size of the data contained in this chunk
        /// </summary>
        public int chunk_size;

        /// <summary>
        /// The raw audio data
        /// </summary>
        public byte[] data;


        /// <summary>
        /// Tries to parse a data chunk from given stream
        /// </summary>
        /// <param name="reader">The binary reader to the opened audio file stream</param>
        /// <returns>Filled <c>DataChunk</c></returns>
        /// <exception cref="Exception"></exception>
        public static DataChunk Parse(BinaryReader reader)
        {

            //Read until data block is reached
            string data_id = "";
            while(!data_id.Contains("data"))
            {
                char[] buff = reader.ReadChars(1);
                data_id += new string(buff);
            }

            DataChunk output = new DataChunk
            {
                chunk_id = data_id,
                chunk_size = reader.ReadInt32()
                //data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position))
            };
            output.data = reader.ReadBytes(output.chunk_size);
            return output;
        }
    }
}