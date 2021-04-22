using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using Silk.NET.Core.Contexts;

namespace HornetEngine.Graphics
{
    public class NativeWindow
    {
        public unsafe WindowHandle* w_handle;
        public static GL gcontext;
        public static Glfw fwcontext;

        protected NativeWindow()
        {
            
        }

        public unsafe bool CreateWindowHandle(int width, int height, string title, WindowMode mode)
        {
            if (fwcontext == null)
            {
                fwcontext = Glfw.GetApi();
                bool init = fwcontext.Init();
                if(!init)
                {
                    throw new Exception("GLFW context init failed");
                }
            }
            fwcontext.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGL);
            fwcontext.WindowHint(WindowHintInt.ContextVersionMajor, 4);
            fwcontext.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            fwcontext.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            fwcontext.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
            fwcontext.WindowHint(WindowHintBool.Resizable, true);
            fwcontext.WindowHint(WindowHintBool.DoubleBuffer, true);
            fwcontext.SetErrorCallback(ErrorCallback);

            switch (mode)
            {
                case WindowMode.FULLSCREEN:
                    CreateWindowFull(title, width, height);
                    break;
                case WindowMode.WINDOWED:
                    CreateWindowWindowed(title, width, height);
                    break;
                case WindowMode.WINDOWED_FULLSCREEN:
                    CreateWindowFullW(title, width, height);
                    break;
            }
            if(this.w_handle == null)
            {
                Debug.WriteLine("Failed to create window");
                return false;
            }
            fwcontext.ShowWindow(w_handle);
            fwcontext.MakeContextCurrent(w_handle);

            if (gcontext == null)
            {
                gcontext = GL.GetApi(fwcontext.GetProcAddress);
            }
            return true;
        }

        private void ErrorCallback(Silk.NET.GLFW.ErrorCode error, string description)
        {
            Console.WriteLine(error);
            Console.WriteLine(description);
        }

        private unsafe void CreateWindowWindowed(String title, int width, int height)
        {
            Glfw context = Glfw.GetApi();
            this.w_handle = context.CreateWindow(width, height, title, null, null);
        }

        private unsafe void CreateWindowFull(String title, int width, int height)
        {
            Glfw context = Glfw.GetApi();
            Monitor* mon = context.GetPrimaryMonitor();
            this.w_handle = context.CreateWindow(width, height, title, mon, null);
        }

        private unsafe void CreateWindowFullW(String title, int width, int height)
        {
            Glfw context = Glfw.GetApi();
            Monitor* mon = context.GetPrimaryMonitor();
            VideoMode* vidmode = context.GetVideoMode(mon);

            context.WindowHint(WindowHintInt.RedBits, vidmode->RedBits);
            context.WindowHint(WindowHintInt.GreenBits, vidmode->GreenBits);
            context.WindowHint(WindowHintInt.BlueBits, vidmode->BlueBits);
            context.WindowHint(WindowHintInt.RefreshRate, vidmode->RefreshRate);
            this.w_handle = context.CreateWindow(width, height, title, mon, null);
        }
    }
}
