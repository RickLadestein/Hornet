using System;
using System.Collections.Generic;
using System.Text;
using Silk.NET.OpenGL;
using System.Numerics;
using Silk.NET.GLFW;

namespace HornetEngine.Graphics
{
    public class Window : NativeWindow
    {
        
        
        public String Title { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public Window(): base() {
            
        }

        public bool Open(String title, int width, int height, bool fullscreen)
        {
            bool result = this.CreateWindowHandle(width, height, title, WindowMode.WINDOWED);
            NativeWindow.gcontext.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            return result;
        }

        public unsafe void Run()
        {
            Glfw context = Glfw.GetApi();
            while (!context.WindowShouldClose(this.w_handle))
            {
                context.PollEvents();
                gcontext.Clear((uint)ClearBufferMask.ColorBufferBit);
                fwcontext.SwapBuffers(this.w_handle);
            }
            context.DestroyWindow(this.w_handle);
            this.w_handle = null;
        }
    }
}
