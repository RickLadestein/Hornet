using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Graphics.Buffers
{
    /// <summary>
    /// Enum describing depth buffer behaviour
    /// </summary>
    public enum DepthFunc
    {
        /// <summary>
        /// New depth values are always written in the depth buffer
        /// </summary>
        ALWAYS = Silk.NET.OpenGL.DepthFunction.Always,

        /// <summary>
        /// New depth values are never written in the depth buffer
        /// </summary>
        NEVER = Silk.NET.OpenGL.DepthFunction.Never,

        /// <summary>
        /// New depth values are only written when less than the value in the depth buffer
        /// </summary>
        LESS = Silk.NET.OpenGL.DepthFunction.Less,

        /// <summary>
        /// New depth values are only written when equal than the value in the depth buffer
        /// </summary>
        EQUAL = Silk.NET.OpenGL.DepthFunction.Equal,

        /// <summary>
        /// New depth values are only written when greater than the value in the depth buffer
        /// </summary>
        GREATER = Silk.NET.OpenGL.DepthFunction.Greater,

        /// <summary>
        /// New depth values are only written when not equal to the value in the depth buffer
        /// </summary>
        NOTEQUAL = Silk.NET.OpenGL.DepthFunction.Notequal,

        /// <summary>
        /// New depth values are only written when less or equal to the value in the depth buffer
        /// </summary>
        LEQUAL = Silk.NET.OpenGL.DepthFunction.Lequal,

        /// <summary>
        /// New depth values are only written when greater or equal to the value in the depth buffer
        /// </summary>
        GEQUAL = Silk.NET.OpenGL.DepthFunction.Gequal,
    }



    public static class DepthBuffer
    {
        /// <summary>
        /// Indication if depth buffer is enabled
        /// </summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// Enables the Depth buffering feature
        /// </summary>
        public static void Enable()
        {
            NativeWindow.GL.DepthMask(true);
            IsEnabled = true;
        }


        /// <summary>
        /// Disables the depth buffering feature
        /// </summary>
        public static void Disable()
        {
            NativeWindow.GL.DepthMask(false);
            IsEnabled = false;
        }

        public static void SetDepthCheckBehaviour(DepthFunc func_enum)
        {
            NativeWindow.GL.DepthFunc((GLEnum)func_enum);
        }

        /// <summary>
        /// Clears the depth buffer if enabled
        /// </summary>
        public static void ClearDepthBuffer()
        {
            if (NativeWindow.GL.IsEnabled(Silk.NET.OpenGL.GLEnum.DepthTest))
            {
                NativeWindow.GL.Clear((uint)Silk.NET.OpenGL.GLEnum.DepthBufferBit);
            }
        }
    }
}
