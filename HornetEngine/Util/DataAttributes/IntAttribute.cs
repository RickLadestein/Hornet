using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace HornetEngine.Graphics
{
    public class IntAttribute : Attribute
    {
        public IntAttribute(String name, uint comps)
        {
            this.Base_type = AttributeType.INT;
            this.Components = comps;
            this.Base_type_size = sizeof(int);
            this.Name = name;
        }

        public void AddData(int data)
        {
            byte[] bts = BitConverter.GetBytes(data);
            this.byte_data.AddRange(bts);
        }

        public void AddData(Vector2i data)
        {
            byte[] bts = new byte[sizeof(int)];
            for (int i = 0; i < 2; i++)
            {
                bts = BitConverter.GetBytes(data[i]);
                this.byte_data.AddRange(bts);
            }
        }

        public void AddData(Vector3i data)
        {
            byte[] bts = new byte[sizeof(int)];
            for (int i = 0; i < 3; i++)
            {
                bts = BitConverter.GetBytes(data[i]);
                this.byte_data.AddRange(bts);
            }
        }

        public void AddData(Vector4i data)
        {
            byte[] bts = new byte[sizeof(int)];
            for (int i = 0; i < 4; i++)
            {
                bts = BitConverter.GetBytes(data[i]);
                this.byte_data.AddRange(bts);
            }
        }

        #region ArrayData
        public void AddData(int[] data)
        {
            unsafe
            {
                int buffersize = sizeof(int);
                byte[] ba = new byte[data.Length * buffersize];
                Buffer.BlockCopy(data, 0, ba, 0, buffersize);
                this.byte_data.AddRange(ba);
            }
        }

        public void AddData(Vector2i[] data)
        {
            unsafe
            {
                int size = Marshal.SizeOf(typeof(Vector2i)) * data.Length;
                byte[] array = new byte[size];
                fixed (void* dp = &data[0])
                {
                    IntPtr ptr = new IntPtr(dp);
                    Marshal.Copy(ptr, array, 0, size);
                    this.byte_data.AddRange(array);
                }
            }
        }

        public void AddData(Vector3i[] data)
        {
            unsafe
            {
                int size = Marshal.SizeOf(typeof(Vector3i)) * data.Length;
                byte[] array = new byte[size];
                fixed(void* dp = &data[0])
                {
                    IntPtr ptr = new IntPtr(dp);
                    Marshal.Copy(ptr, array, 0, size);
                    this.byte_data.AddRange(array);
                }
            }
        }

        public void AddData(Vector4i[] data)
        {
            unsafe
            {
                int size = Marshal.SizeOf(typeof(Vector4i)) * data.Length;
                byte[] array = new byte[size];
                fixed (void* dp = &data[0])
                {
                    IntPtr ptr = new IntPtr(dp);
                    Marshal.Copy(ptr, array, 0, size);
                    this.byte_data.AddRange(array);
                }
            }
        }
        #endregion

    }
}
