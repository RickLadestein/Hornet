using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace HornetEngine.Graphics
{
    public class DoubleAttribute : Attribute
    {
        public DoubleAttribute(String name, uint comps)
        {
            this.Base_type = AttributeType.DOUBLE;
            this.Components = comps;
            this.Base_type_size = sizeof(double);
            this.Datapoints = 0;
            this.Name = name;
        }

        public void AddData(double data)
        {
            byte[] bts = BitConverter.GetBytes(data);
            this.byte_data.AddRange(bts);
            this.Datapoints += 1;
        }

        public void AddData(Vector2d data)
        {
            byte[] bts = new byte[sizeof(float)];
            for (int i = 0; i < 2; i++)
            {
                bts = BitConverter.GetBytes(data[i]);
                this.byte_data.AddRange(bts);
                this.Datapoints += 1;
            }
        }

        public void AddData(Vector3d data)
        {
            byte[] bts = new byte[sizeof(float)];
            for (int i = 0; i < 3; i++)
            {
                bts = BitConverter.GetBytes(data[i]);
                this.byte_data.AddRange(bts);
                this.Datapoints += 1;
            }
        }

        public void AddData(Vector4d data)
        {
            byte[] bts = new byte[sizeof(float)];
            for (int i = 0; i < 4; i++)
            {
                bts = BitConverter.GetBytes(data[i]);
                this.byte_data.AddRange(bts);
                this.Datapoints += 1;
            }
        }

        #region ArrayData
        public void AddData(double[] data)
        {
            unsafe
            {
                int buffersize = sizeof(double);
                byte[] ba = new byte[data.Length * buffersize];
                Buffer.BlockCopy(data, 0, ba, 0, buffersize);
                this.byte_data.AddRange(ba);
                this.Datapoints += (uint)data.Length;
            }
        }

        public void AddData(Vector2d[] data)
        {
            unsafe
            {
                int size = Marshal.SizeOf(typeof(Vector2d)) * data.Length;
                byte[] array = new byte[size];
                fixed (void* dp = &data[0])
                {
                    IntPtr ptr = new IntPtr(dp);
                    Marshal.Copy(ptr, array, 0, size);
                    this.byte_data.AddRange(array);
                }
            }
        }

        public void AddData(Vector3d[] data)
        {
            unsafe
            {
                int size = Marshal.SizeOf(typeof(Vector3d)) * data.Length;
                byte[] array = new byte[size];
                fixed (void* dp = &data[0])
                {
                    IntPtr ptr = new IntPtr(dp);
                    Marshal.Copy(ptr, array, 0, size);
                    this.byte_data.AddRange(array);
                }
            }
        }

        public void AddData(Vector4d[] data)
        {
            unsafe
            {
                int size = Marshal.SizeOf(typeof(Vector4d)) * data.Length;
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
