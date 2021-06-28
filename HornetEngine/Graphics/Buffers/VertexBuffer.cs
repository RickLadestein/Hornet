using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Silk.NET.OpenGL;

namespace HornetEngine.Graphics.Buffers
{
    public enum ElementType
    {
        POINTS = Silk.NET.OpenGL.PrimitiveType.Points,
        LINES = Silk.NET.OpenGL.PrimitiveType.Lines,
        TRIANGLES = Silk.NET.OpenGL.PrimitiveType.Triangles
    }

    public class VertexBuffer : IDisposable
    {
        /// <summary>
        /// The current error code
        /// </summary>
        public String Error { get; private set; }

        /// <summary>
        /// The handle to the VAO data buffer in the GPU
        /// </summary>
        public uint Handle { get; private set; }

        /// <summary>
        /// The type of primitives that make up the mesh
        /// </summary>
        public ElementType PrimitiveType { get; private set; }

        /// <summary>
        /// The amount of vertices in the VertexBuffer
        /// </summary>
        public uint VertexCount { get; private set; }


        private int current_vao_attrib;

        private uint vbo_handle;

        private uint primitive_vertex_count;

        /// <summary>
        /// The constructor of the VertexBuffer
        /// </summary>
        public VertexBuffer()
        {
            this.Error = string.Empty;
            this.Handle = 0;
            this.vbo_handle = 0;
            this.current_vao_attrib = 0;
            this.VertexCount = 0;
            this.primitive_vertex_count = 1;
            this.PrimitiveType = ElementType.POINTS;
        }

        /// <summary>
        /// Bind the buffer
        /// </summary>
        public void Bind()
        {
            NativeWindow.GL.BindVertexArray(this.Handle);
        }

        /// <summary>
        /// Unbind the buffer
        /// </summary>
        public void Unbind()
        {
            NativeWindow.GL.BindVertexArray(0);
        }

        /// <summary>
        /// A function which can be used to initialize the buffer data
        /// </summary>
        /// <param name="attributes">The atrributes which should be bound</param>
        /// <param name="ptype">The way the data should be interpreted by the GPU</param>
        public void BufferData(AttributeStorage attributes, ElementType ptype)
        {
            if (this.Handle == 0 || this.vbo_handle == 0)
            {
                this.Error = "Could not load data into buffers: Buffers not initialised";
                return;
            }
            //Allign all data
            uint total_byte_count = (uint)attributes.GetTotalByteCount();
            int vbo_copy_index = 0;
            byte[] vbo_buff = new byte[total_byte_count];
            foreach(Attribute at in attributes)
            {
                at.byte_data.CopyTo(vbo_buff, vbo_copy_index);
                vbo_copy_index += at.byte_data.Count;
            }

            //Gen and fill the GPU vbo buffer
            NativeWindow.GL.BindBuffer(GLEnum.ArrayBuffer, vbo_handle);
            unsafe
            {
                fixed (void* d_ptr = &vbo_buff[0])
                {
                    NativeWindow.GL.BufferData(GLEnum.ArrayBuffer, total_byte_count, d_ptr, GLEnum.StaticDraw);
                }
            }

            NativeWindow.GL.BindVertexArray(this.Handle);
            if(!attributes.ValidateDataAlignment())
            {
                this.Error = "Attribute data was misaligned, please ensure that all attributes have the same datapoint size";
                return;
            }

            unsafe {
                this.VertexCount = (uint)attributes[0].GetDatapointCount();
                int offset = 0;
                uint stride = 0;
                foreach(Attribute at in attributes)
                {
                    int comp_count = (int)at.Components;
                    stride = at.Base_type_size * at.Components;
                    GLEnum base_type = (GLEnum)at.Base_type;
                    void* offset_ptr = (void*)offset;
                    uint attrib = (uint)this.current_vao_attrib;
                    NativeWindow.GL.VertexAttribPointer(attrib, comp_count, base_type, false, stride, offset_ptr);
                    NativeWindow.GL.EnableVertexAttribArray(attrib);
                    offset += at.byte_data.Count;
                    this.current_vao_attrib += 1;
                }
            }

            this.SetPrimitiveType(ptype);

            NativeWindow.GL.BindBuffer(GLEnum.ArrayBuffer, 0);
            NativeWindow.GL.BindVertexArray(0);
        }

        /// <summary>
        /// A function which sets the primitive type
        /// </summary>
        /// <param name="eltype"></param>
        public void SetPrimitiveType(ElementType eltype)
        {
            this.Error = String.Empty;
            switch(eltype)
            {
                case ElementType.POINTS:
                    primitive_vertex_count = 1;
                    break;
                case ElementType.LINES:
                    primitive_vertex_count = 2;
                    break;
                case ElementType.TRIANGLES:
                    primitive_vertex_count = 3;
                    break;
            }
            this.PrimitiveType = eltype;

            if(VertexCount % primitive_vertex_count != 0)
            {
                this.Error = "Vertex datapoints in vertexbuffer dont match the size of the requested ElementType";
            }
        }

        /// <summary>
        /// A function which initializes the buffers
        /// </summary>
        public void InitialiseBuffers()
        {
            if(this.Handle != 0 || this.vbo_handle != 0)
            {
                this.Error = "Could not initialize buffers: GPU buffers for this object already exists";
            }

            this.Handle = NativeWindow.GL.GenVertexArray();
            this.vbo_handle = NativeWindow.GL.GenBuffer();
        }

        /// <summary>
        /// A function which destroys the buffers
        /// </summary>
        public void DestroyBuffers()
        {
            if(this.Handle != 0)
            {
                NativeWindow.GL.DeleteBuffer(this.Handle);
                this.Handle = 0;
            }

            if(this.vbo_handle != 0)
            {
                NativeWindow.GL.DeleteVertexArray(this.vbo_handle);
            }

            this.current_vao_attrib = 0;
        }

        /// <summary>
        /// A function which disposes of the vertex buffers
        /// </summary>
        public void Dispose()
        {
            this.DestroyBuffers();
        }
    }
}
