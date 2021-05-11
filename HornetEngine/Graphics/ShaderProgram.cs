using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Silk.NET.OpenGL;
using System.Numerics;

namespace HornetEngine.Graphics
{
    public enum ShaderProgramStatus
    {
        UNINITIALISED,
        AQUIRING_HANDLE,
        LINKING,
        READY
    }

    public class ShaderProgram : IDisposable
    {
        /// <summary>
        /// The internal GPU handle for the ShaderProgram
        /// </summary>
        public uint Handle { get; private set; }

        /// <summary>
        /// The error string containing the error log of the ShaderProgram
        /// </summary>
        public String Error { get; private set; }

        /// <summary>
        /// The current status of the ShaderProgram
        /// </summary>
        public ShaderProgramStatus Status { get; private set; }

        /// <summary>
        /// Creates a new instance of ShaderProgram with vertex and fragment shader
        /// </summary>
        /// <param name="vertex">Vertex shader program</param>
        /// <param name="fragment">Fragment shader program</param>
        public ShaderProgram(VertexShader vertex, FragmentShader fragment)
        {
            this.Status = ShaderProgramStatus.UNINITIALISED;
            if (vertex.Status != ShaderStatus.READY || fragment.Status != ShaderStatus.READY)
            {
                Error = "One of the shaders was not compiled correctly: see individual shader error logs for detail";
                return;
            }

            this.Status = ShaderProgramStatus.AQUIRING_HANDLE;
            this.Handle = NativeWindow.GL.CreateProgram();
            if (Handle == 0)
            {
                Error = "OpenGL was not able to create ShaderProgram handle";
                return;
            }

            this.Status = ShaderProgramStatus.LINKING;
            NativeWindow.GL.AttachShader(this.Handle, vertex.Handle);
            NativeWindow.GL.AttachShader(this.Handle, fragment.Handle);
            NativeWindow.GL.LinkProgram(this.Handle);
            String log = GetProgramInfoLog();
            if(log.Length > 0)
            {
                this.Error = log;
                return;
            }
            this.Status = ShaderProgramStatus.READY;
        }

        /// <summary>
        /// Creates a new instance of ShaderProgram with vertex, geometry and fragment shader
        /// </summary>
        /// <param name="vertex">Vertex shader program</param>
        /// <param name="geometry">Geometry shader program</param>
        /// <param name="fragment">Fragment shader program</param>
        public ShaderProgram(VertexShader vertex, GeometryShader geometry, FragmentShader fragment)
        {
            this.Status = ShaderProgramStatus.UNINITIALISED;
            if (vertex.Status != ShaderStatus.READY || 
                fragment.Status != ShaderStatus.READY || 
                geometry.Status != ShaderStatus.READY)
            {
                Error = "One of the shaders was not compiled correctly: see individual shader error logs for detail";
                return;
            }

            this.Status = ShaderProgramStatus.AQUIRING_HANDLE;
            this.Handle = NativeWindow.GL.CreateProgram();
            if (Handle == 0)
            {
                Error = "OpenGL was not able to create ShaderProgram handle";
                return;
            }

            this.Status = ShaderProgramStatus.LINKING;
            NativeWindow.GL.AttachShader(this.Handle, vertex.Handle);
            NativeWindow.GL.AttachShader(this.Handle, geometry.Handle);
            NativeWindow.GL.AttachShader(this.Handle, fragment.Handle);
            NativeWindow.GL.LinkProgram(this.Handle);
            String log = GetProgramInfoLog();
            if (log.Length > 0)
            {
                this.Error = log;
                return;
            }
            this.Status = ShaderProgramStatus.READY;
        }

        /// <summary>
        /// Binds the ShaderProgram as the currently active program in the GPU.
        /// There can only be a single program bound to the GPU 
        /// so this call switches the active target.
        /// </summary>
        public void Bind()
        {
            NativeWindow.GL.UseProgram(this.Handle);
        }

        /// <summary>
        /// Unbinds all shader programs from the GPU
        /// </summary>
        public static void UnbindAll()
        {
            NativeWindow.GL.UseProgram(0);
        }

        /// <summary>
        /// Retrieves the current info log of the ShaderProgram 
        /// that indicates information such as errors and warnings during linking
        /// </summary>
        /// <returns>String containing the current shader info log</returns>
        private String GetProgramInfoLog()
        {
            if (Handle == 0)
            {
                return "ShaderProgram handle was not initialised";
            }

            String log = NativeWindow.GL.GetProgramInfoLog(this.Handle);
            if (log.Length > 0)
            {
                return $"ShaderProgram linking failed: \n {log}";
            }
            return String.Empty;
        }


        #region Uniforms

        public void SetUniform(string location, bool value)
        {
            int loc = NativeWindow.GL.GetUniformLocation(this.Handle, location);
            if (loc != -1)
            {
                int rep = value ? 255 : 0;
                NativeWindow.GL.Uniform1(loc, rep);
            }
        }

        public void SetUniform(string location, int value)
        {
            int loc = NativeWindow.GL.GetUniformLocation(this.Handle, location);
            if (loc != -1)
            {
                NativeWindow.GL.Uniform1(loc, value);
            }
        }

        public void SetUniform(string location, uint value)
        {
            int loc = NativeWindow.GL.GetUniformLocation(this.Handle, location);
            if (loc != -1)
            {
                NativeWindow.GL.Uniform1(loc, value);
            }
        }

        public void SetUniform(string location, float value)
        {
            int loc = NativeWindow.GL.GetUniformLocation(this.Handle, location);
            if(loc != -1)
            {
                NativeWindow.GL.Uniform1(loc, value);
            }
        }

        public void SetUniform(string location, double value)
        {
            int loc = NativeWindow.GL.GetUniformLocation(this.Handle, location);
            if (loc != -1)
            {
                NativeWindow.GL.Uniform1(loc, value);
            }
        }

        public void SetUniform(string location, Vector2 value)
        {
            int loc = NativeWindow.GL.GetUniformLocation(this.Handle, location);
            if (loc != -1)
            {
                NativeWindow.GL.Uniform2(loc, value);
            }
        }

        public void SetUniform(string location, Vector3 value)
        {
            int loc = NativeWindow.GL.GetUniformLocation(this.Handle, location);
            if (loc != -1)
            {
                NativeWindow.GL.Uniform3(loc, value);
            }
        }

        public void SetUniform(string location, Vector4 value)
        {
            int loc = NativeWindow.GL.GetUniformLocation(this.Handle, location);
            if (loc != -1)
            {
                NativeWindow.GL.Uniform4(loc, value);
            }
        }

        public unsafe void SetUniform(string location, Matrix4x4 matrix)
        {
            int loc = NativeWindow.GL.GetUniformLocation(this.Handle, location);
            if (loc != -1)
            {
                NativeWindow.GL.UniformMatrix4(loc, 1, false, (float*) &matrix);
            }
        }


        #endregion

        /// <summary>
        /// Deletes this ShaderProgram from the GPU unmanaged resources
        /// </summary>
        public void Dispose()
        {
            if(this.Handle != 0)
            {
                NativeWindow.GL.DeleteProgram(this.Handle);
            }
        }
    }

    public enum ShaderStatus
    {
        UNINITIALISED,
        AQUIRING_HANDLE,
        IMPORTING_SOURCE_CODE,
        COMPILING,
        READY
    }

    public abstract class Shader : IDisposable
    {
        /// <summary>
        /// The shader internal program handle
        /// </summary>
        public uint Handle { get; protected set; }

        /// <summary>
        /// The type of shader [VERTEX, GEOMETRY, FRAGMENT...]
        /// </summary>
        public ShaderType Type { get; protected set; }

        /// <summary>
        /// The error string containing errors and warnings of the Shader during compile time
        /// </summary>
        public String Error { get; protected set; }

        /// <summary>
        /// Indication of the current shader program status
        /// </summary>
        public ShaderStatus Status { get; protected set; }

        /// <summary>
        /// Constructs a new instance of Shader with specified parameters
        /// </summary>
        /// <param name="type">The type of shader [Vertex, Geometry, Fragment...]</param>
        /// <param name="sourcefile">The path to the shader source code</param>
        public Shader(ShaderType type,  string sourcefile)
        {
            this.Type = type;
            this.Status = ShaderStatus.UNINITIALISED;

            this.Status = ShaderStatus.AQUIRING_HANDLE;
            this.Handle = NativeWindow.GL.CreateShader(type);
            if(this.Handle == 0)
            {
                this.Error = "OpenGL was not able to create shader handle";
                return;
            }

            this.Status = ShaderStatus.IMPORTING_SOURCE_CODE;
            String code = Shader.ReadFromFile(sourcefile);
            if(code.Length == 0)
            {
                this.Error = "Source code import has failed or the file was empty";
                return;
            }

            this.Status = ShaderStatus.COMPILING;
            bool succes = this.CompileShader(code);

            if(succes)
            {
                this.Status = ShaderStatus.READY;
            }
        }


        /// <summary>
        /// Retrieves the current info log of the shader 
        /// that indicates information such as errors and warnings during shader compile time
        /// </summary>
        /// <returns>String containing the current shader info log</returns>
        private String GetInfoLog()
        {
            if (Handle == 0)
            {
                return "Shader handle was not initialised";
            }

            String log = NativeWindow.GL.GetShaderInfoLog(this.Handle);
            if(log.Length > 0)
            {
                return $"{Type} Shader compilation failed: \n {log}";
            }
            return String.Empty;
        }

        /// <summary>
        /// Compiles the source code and links it to the shader
        /// </summary>
        /// <param name="src">The Shader source code</param>
        /// <returns>Boolean indicating compile status; <c>true</c> if success | <c>false</c> if error</returns>
        private bool CompileShader(string src)
        {
            if (this.Handle == 0)
            {
                this.Error = "Shader handle was not initialised";
                return false;
            }
            if (src.Length == 0)
            {
                this.Error = "Shader source code was empty";
                return false;
            }

            NativeWindow.GL.ShaderSource(this.Handle, src);
            NativeWindow.GL.CompileShader(this.Handle);

            string log = GetInfoLog();
            if (log.Length > 0)
            {
                this.Error = log;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to read shaderprogram source code from a file
        /// </summary>
        /// <param name="file">The absolute path to the shader file</param>
        /// <returns>String containing the shader source code</returns>
        private static String ReadFromFile(string file)
        {
            StreamReader rd = null;
            try
            {
                rd = new StreamReader(file);
                String output = rd.ReadToEnd();
                return output;

            } catch(Exception ex)
            {
                Console.WriteLine($"Reading of file {file} failed: {ex.Message}");
                return string.Empty;
            } finally
            {
                rd?.Close();
            }
        }

        /// <summary>
        /// Deletes this Shader from the GPU unmanaged resources
        /// </summary>
        public void Dispose()
        {
            if(this.Handle != 0)
            {
                NativeWindow.GL.DeleteShader(this.Handle);
            }
        }
    }

    public class VertexShader : Shader
    {
        public VertexShader(string file) : base(ShaderType.VertexShader, file) {}
    }

    public class GeometryShader : Shader
    {
        public GeometryShader(string file) : base(ShaderType.GeometryShader, file) { }
    }

    public class FragmentShader : Shader
    {
        public FragmentShader(string file) : base(ShaderType.FragmentShader, file) { }
    }

    
}
