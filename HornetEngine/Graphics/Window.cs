using System;
using System.Collections.Generic;
using System.Text;
using Silk.NET.OpenGL;
using System.Numerics;
using Silk.NET.GLFW;
using HornetEngine.Util.Drivers;
using HornetEngine.Util;
using System.Threading;
using HornetEngine.Input;
using HornetEngine.Graphics.Buffers;

namespace HornetEngine.Graphics
{
    public class Window : NativeWindow
    {
        public Mouse Mouse { get; private set; }
        public Keyboard Keyboard { get; private set; }
        public TouchPanel Touch_panel { get; private set; }


        public delegate void WindowRefreshFunc();
        public delegate void WindowFixedUpdateFunc();
        public delegate void WindowMoveFunc(Vector2 newpos);
        public delegate void WindowResizeFunc(Vector2 newsize);
        public delegate void WindowFocusFunc(bool focussed);
        public delegate void WindowCloseFunc();

        public event WindowRefreshFunc Redraw;
        public event WindowMoveFunc Move;
        public event WindowResizeFunc Resize;
        public event WindowFocusFunc Focus;
        public event WindowCloseFunc Close;
        public event WindowFixedUpdateFunc FixedUpdate;

        private Thread fixed_update_thread;

        private double start_time;
        private double end_time;
        private float last_frame_time;
        private bool alive;
        private float fixed_update_frequency;

        /// <summary>
        /// Instantiates a new window object with base parameters
        /// </summary>
        public Window(): base() {
            DirectoryManager.InitResourceDir();
            start_time = 0.0d;
            end_time = 0.0d;
            last_frame_time = 0.0f;
            alive = false;
            fixed_update_frequency = 1.0f / 60.0f;
            fixed_update_thread = new Thread(() => { FixedUpdateFunc(); });
        }

        /// <summary>
        /// Opens a new application window on the current thread with specified title and size parameters
        /// </summary>
        /// <param name="title">The title of the application window</param>
        /// <param name="width">The width of the application window in pixels</param>
        /// <param name="height">The height of the application window in pixels</param>
        /// <param name="fullscreen">Fullscreen specifier, false: undecorated window, true: decorated window</param>
        /// <returns>Window creation succes status, false: window creation failed, true: window creation succesfull</returns>
        public bool Open(String title, int width, int height, WindowMode mode)
        {
            bool result = this.CreateWindowHandle(width, height, title, mode);
            GL.ClearColor(0.45f, 0.45f, 0.45f, 1.0f);
            unsafe
            {
                this.Mouse = new Mouse(this.w_handle);
                this.Keyboard = new Keyboard(this.w_handle);
                this.Touch_panel = new TouchPanel(this.touch_driver);
            }

            this.Redraw += Scene.Instance.GetRefreshFunc();

            //init default opengl behaviour
            NativeWindow.GL.Enable(GLEnum.CullFace);
            NativeWindow.GL.CullFace(CullFaceMode.Back);
            NativeWindow.GL.Enable(GLEnum.DepthTest);
            DepthBuffer.Enable();
            DepthBuffer.SetDepthCheckBehaviour(DepthFunc.LESS);


            fixed_update_thread.Start();

            return result;
        }

        /// <summary>
        /// Start the application window refresh loop and processes
        /// </summary>
        public void Run()
        {
            while (!this.ShouldClose())
            {
                start_time = this.GetAliveTime();
                this.PollEvents();
                this.ClearBuffer(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                
                Scene.Instance.UpdateScene();
                this.Redraw?.Invoke();

                this.SwapBuffers();
                end_time = this.GetAliveTime();
                Time.FrameDelta = (float) (end_time - start_time);

            }
        }

        private void FixedUpdateFunc()
        {
            while(alive)
            {
                DateTime begin = DateTime.Now;
                Scene.Instance.UpdateFixed();
                FixedUpdate?.Invoke();
                DateTime end = DateTime.Now;
                TimeSpan delta = end - begin;
                if(delta.TotalSeconds < fixed_update_frequency)
                {
                    double wait_time = (double)fixed_update_frequency - delta.TotalSeconds;
                    Thread.Sleep((int)wait_time * 1000);
                }
            }
        }

        /// <summary>
        /// Sets the FixedUpdate frequency of the FixedUpdate thread
        /// </summary>
        /// <param name="newfreq">The new frequency in Hz</param>
        public void SetFixedUpdateFrequency(float newfreq)
        {
            this.fixed_update_frequency = 1.0f / newfreq;
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
