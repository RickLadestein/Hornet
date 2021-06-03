using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using Silk.NET.Core.Contexts;
using HornetEngine.Util.Exceptions;
using System.Numerics;
using HornetEngine.Input;
using HornetEngine.Util.Drivers;

namespace HornetEngine.Graphics
{
    public abstract class NativeWindow : IDisposable
    {
        protected unsafe WindowHandle* w_handle;
        protected unsafe GlfwNativeWindow n_window;
        protected TouchDriver touch_driver;
        
        private static GL gcontext;
        public static GL GL {
            get
            {
                if (gcontext == null)
                {
                    throw new Exception("OpenGL context needs to be initialised first");
                }
                else
                {
                    return gcontext;
                }
            } 
        }

        private static Glfw fwcontext;
        public static Glfw GLFW
        {
            get
            {
                if(fwcontext == null)
                {
                    throw new Exception("GLFW context needs to be initialised first");
                } else
                {
                    return fwcontext;
                }
            }
        }

        private String _title;
        private Vector2 _size;
        private Vector2 _pos;

        public String Title
        {
            get
            {
                return _title;
            }
            set
            {
                this.SetTitle(value);
            }
        }

        public Vector2 Size
        {
            get
            {
                return _size;
            }
            set
            {
                this.SetSize((int)Size.X, (int)Size.Y);
            }
        }

        public Vector2 Pos
        {
            get
            {
                return _pos;
            }
            set
            {
                this.SetPosition((uint)value.X, (uint)value.Y);
            }
        }
        protected NativeWindow()
        {
            this._title = "";
            this._pos = new Vector2(0.0f);
            this._size = new Vector2(0.0f);
        }

        public unsafe void SetTitle(String title)
        {
            EnsureContextAndWindow();
            fwcontext.SetWindowTitle(w_handle, title);
            _title = title;
        }

        public unsafe void SetSize(int width, int height)
        {
            EnsureContextAndWindow();
            fwcontext.SetWindowSize(w_handle, width, height);
            _size = new Vector2(width, height);
        }

        public unsafe void SetPosition(uint x_pos, uint y_pos)
        {
            EnsureContextAndWindow();
            Monitor* mon = fwcontext.GetPrimaryMonitor();
            if(mon == null)
            {
                throw new NativeWindowException("Could not find primary monitor");
            } else
            {
                fwcontext.GetMonitorWorkarea(mon, out int x, out int y, out int width, out int height);
                if (x_pos > width || y_pos > height)
                {
                    throw new NativeWindowException($"New window coordinates [{x_pos}|{y_pos}] are outside the monitor bounds [{width}|{height}]");
                }
                else
                {
                    fwcontext.SetWindowPos(w_handle, (int)x_pos, (int)y_pos);
                    this._pos = new Vector2(x_pos, y_pos);
                }
            }
        }

        public unsafe double GetAliveTime()
        {
            EnsureContextAndWindow();
            return fwcontext.GetTime();
        }

        public unsafe bool ShouldClose()
        {
            EnsureContextAndWindow();
            return fwcontext.WindowShouldClose(w_handle);
        }

        public unsafe void SetDrawArea(uint x, uint y, uint width, uint height)
        {
            EnsureContextAndWindow();

            gcontext.Viewport((int)x, (int)y, width, height);
        }

        protected unsafe void SwapBuffers()
        {
            EnsureContextAndWindow();
            fwcontext.SwapBuffers(w_handle);
        }

        protected unsafe void ClearBuffer(ClearBufferMask mask)
        {
            EnsureContextAndWindow();
            gcontext.Clear((uint)mask);
        }

        protected unsafe void PollEvents()
        {
            EnsureContextAndWindow();
            fwcontext.PollEvents();
        }

        protected abstract void OnWindowSizeChanged(int width, int height);
        protected abstract void OnWindowPosChanged(int x, int y);
        protected abstract void OnWindowFocusChanged(bool isfocus);
        protected abstract void OnWindowCloseRequested();
        protected abstract void OnWindowIconify(bool iconified);
        protected abstract void OnWindowMaximize(bool maximized);

        private unsafe void EnsureContextAndWindow()
        {
            if (fwcontext == null || gcontext == null)
            {
                throw new ContextException(fwcontext, "GLFW or GL context not initialised");
            }

            if (w_handle == null)
            {
                throw new NativeWindowException("Window was not created");
            }
        }

        private void ErrorCallback(Silk.NET.GLFW.ErrorCode error, string description)
        {
            Console.WriteLine(error);
            Console.WriteLine(description);
        }
        #region Window_Creation
        protected unsafe bool CreateWindowHandle(int width, int height, string title, WindowMode mode)
        {
            if (fwcontext == null)
            {
                fwcontext = Glfw.GetApi();
                bool init = fwcontext.Init();
                if(!init)
                {
                    throw new ContextInitException(fwcontext, "");
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
                if(gcontext == null)
                {
                    throw new ContextInitException(gcontext, "Could not bind to GLFW PROC");
                }
            }

            this._size = new Vector2(width, height);
            this._title = title;
            fwcontext.GetWindowPos(w_handle, out int x_pos, out int y_pos);
            this._pos = new Vector2(x_pos, y_pos);


            fwcontext.SetWindowPosCallback(w_handle, NativeWindowPosChanged);
            fwcontext.SetWindowSizeCallback(w_handle, NativeWindowSizeChanged);
            fwcontext.SetWindowFocusCallback(w_handle, NativeWindowFocusChanged);
            fwcontext.SetWindowCloseCallback(w_handle, NativeWindowCloseRequested);
            fwcontext.SetWindowIconifyCallback(w_handle, NativeWindowIconify);
            fwcontext.SetWindowMaximizeCallback(w_handle, NativeWindowMaximize);
            //fwcontext.SwapInterval(0);

            n_window = new GlfwNativeWindow(fwcontext, w_handle);
            touch_driver = new TouchDriver(ref n_window);

            return true;
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
        #endregion

        #region native_callbacks
        private unsafe void NativeWindowPosChanged(WindowHandle* wnd, int x, int y)
        {
            OnWindowPosChanged(x, y);
        }

        private unsafe void NativeWindowSizeChanged(WindowHandle* wnd, int width, int height)
        {
            OnWindowSizeChanged(width, height);
        }

        private unsafe void NativeWindowFocusChanged(WindowHandle* wnd, bool focus)
        {
            OnWindowFocusChanged(focus);
        }

        private unsafe void NativeWindowCloseRequested(WindowHandle* wnd)
        {
            OnWindowCloseRequested();
        }

        private unsafe void NativeWindowIconify(WindowHandle* wnd, bool iconified)
        {
            OnWindowIconify(iconified);
        }

        private unsafe void NativeWindowMaximize(WindowHandle* wnd, bool maximized)
        {
            OnWindowMaximize(maximized);
        }

        #endregion native_callbacks
        public unsafe void Dispose()
        {
            if(w_handle != null)
            {
                fwcontext.DestroyWindow(w_handle);
            }
            
            if (fwcontext != null)
            {
                fwcontext.Terminate();
            }
        }
    }
}
