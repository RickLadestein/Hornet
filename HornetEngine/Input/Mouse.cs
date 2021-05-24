using System;
using System.Collections.Generic;
using System.Text;
using Silk.NET.GLFW;
using HornetEngine.Graphics;

namespace HornetEngine.Input
{
    public class Mouse
    {
        public delegate void MouseMoveFunc();

        public event MouseMoveFunc Moved;
        public unsafe Mouse(WindowHandle* wh)
        {
        }
    }
}
