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
        public delegate void WindowRefreshFunc(float timestep);
        public delegate void WindowMoveFunc(Vector2 newpos);
        public delegate void WindowResizeFunc(Vector2 newsize);
        public delegate void WindowFocusFunc(bool focussed);
        public delegate void WindowCloseFunc();

        public event WindowRefreshFunc Redraw;
        public event WindowMoveFunc Move;
        public event WindowResizeFunc Resize;
        public event WindowFocusFunc Focus;
        public event WindowCloseFunc Close;

        private double start_time;
        private double end_time;
        private float last_frame_time;
        public Window(): base() {
            start_time = 0.0d;
            end_time = 0.0d;
            last_frame_time = 0.0f;
        }

        public bool Open(String title, int width, int height, bool fullscreen)
        {
            bool result = this.CreateWindowHandle(width, height, title, WindowMode.WINDOWED);
            NativeWindow.gcontext.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            return result;
        }

        public void Run()
        {
            while (!this.ShouldClose())
            {
                start_time = this.GetAliveTime();
                this.PollEvents();
                this.ClearBuffer(ClearBufferMask.ColorBufferBit);

                this.Redraw?.Invoke(last_frame_time);

                this.SwapBuffers();
                end_time = this.GetAliveTime();
                last_frame_time = (float) (end_time - start_time);
            }
        }



        protected override void OnWindowSizeChanged(int width, int height)
        {
            Resize?.Invoke(new Vector2(width, height));
        }

        protected override void OnWindowPosChanged(int x, int y)
        {
            Move?.Invoke(new Vector2(x, y));
        }

        protected override void OnWindowFocusChanged(bool isfocus)
        {
            Focus?.Invoke(isfocus);
        }

        protected override void OnWindowCloseRequested()
        {
            Close?.Invoke();
        }

        protected override void OnWindowIconify(bool iconified)
        {
            return;
        }

        protected override void OnWindowMaximize(bool maximized)
        {
            return;
        }
    }
}
