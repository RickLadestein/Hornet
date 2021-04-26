using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Silk.NET.GLFW;
using Silk.NET.Core;
using System.Security.Permissions;

namespace HornetEngine.Input
{
    public enum RegisterTouchFlags
    {
        TWF_FINETOUCH = 0x00000001,
        TWF_WANTPALM = 0x00000002
    }

    public enum TouchEventFlags
    {
        TOUCHEVENTF_MOVE = 0x0001,
        TOUCHEVENTF_DOWN = 0x0002,
        TOUCHEVENTF_UP = 0x0004
    }

    public struct TOUCH_STRUCT
    {
        long x;
        long y;
        IntPtr hSource;
        ulong dwID;
        ulong dwFlags;
        ulong dwMask;
        ulong dwTime;
        IntPtr dwExtraInfo;
        ulong cxContact;
        ulong cyContact;
    }

    class TouchPanel : IDisposable
    {

        private GlfwContext context;
        private IntPtr HWnd;
        private static IntPtr oldproc;

        private static readonly uint WM_TOUCH = 0x0240;

        private bool bound;
        private bool registered_touch;

        delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        public unsafe TouchPanel(Glfw api, ref GlfwNativeWindow wnd)
        {
            GetWindowPtr(ref wnd);
            LinkToWin32();
            //if (wnd.Win32.HasValue)
            //{
            //    IntPtr HInstance = wnd.Win32.Value.HInstance;
            //    IntPtr HWnd = wnd.Win32.Value.Hwnd;
            //    bool result = RegisterTouchWindow(HWnd, 2);
            //    WndProcDelegate procfunc = new WndProcDelegate(WndProc);
            //    IntPtr del = Marshal.GetFunctionPointerForDelegate(procfunc);
            //    oldproc = SetWindowLongPtr(HWnd, -4, del);
            //}
        }

        private unsafe void GetWindowPtr(ref GlfwNativeWindow wnd)
        {
            if (wnd.Win32.HasValue)
            {
                IntPtr HWnd = wnd.Win32.Value.Hwnd;
            } else
            {
                throw new Exception("Could not capture native window ptr");
            }
        }

        private unsafe void LinkToWin32()
        {
            registered_touch = RegisterTouchWindow(HWnd, (ulong)RegisterTouchFlags.TWF_WANTPALM);
            if(registered_touch)
            {
                WndProcDelegate procfunc = new WndProcDelegate(WndProc);
                IntPtr del = Marshal.GetFunctionPointerForDelegate(procfunc);
                oldproc = SetWindowLongPtr(HWnd, -4, del);
            }
        }

        static unsafe IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam)
        {
            if(msg == WM_TOUCH)
            {
                Console.WriteLine("Touch Event");
                long count = wparam.ToInt64();
                IntPtr test = Marshal.AllocHGlobal((int)(sizeof(TOUCH_STRUCT) * count));
                bool succes = GetTouchInputInfo(lparam, (uint)count, test, (uint)count);
                if(succes)
                {
                    List<TOUCH_STRUCT> touch_events = new List<TOUCH_STRUCT>();
                    TOUCH_STRUCT* ptr = (TOUCH_STRUCT*) test.ToPointer();
                    for(int i = 0; i < (int)count; i++)
                    {
                        TOUCH_STRUCT ts = ptr[i];
                        touch_events.Add(ts);
                    }
                    CloseTouchInputHandle(lparam);
                }
            }
            return (IntPtr)CallWindowProc(oldproc, hwnd, msg, wparam, lparam);
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool RegisterTouchWindow(IntPtr HWND, ulong flags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnregisterTouchWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowLongPtr(IntPtr HWND, int index, IntPtr func);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetTouchInputInfo(IntPtr lParam, uint input_count, IntPtr arr_start, uint arr_size);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseTouchInputHandle(IntPtr hTouchInput);

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
