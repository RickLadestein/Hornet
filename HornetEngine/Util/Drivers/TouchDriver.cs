using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Silk.NET.GLFW;
using Silk.NET.Core;
using System.Security.Permissions;
using System.Numerics;

namespace HornetEngine.Util.Drivers
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
        TOUCHEVENTF_UP = 0x0004,
        TOUCHEVENTF_INRANGE = 0x0008,
        TOUCHEVENTF_PRIMARY = 0x0010,
        TOUCHEVENTF_NOCOALESCE = 0x0020,
        TOUCHEVENTF_PEN = 0x0040,
        TOUCHEVENTF_PALM = 0x0080
    }

    public struct TOUCH_STRUCT
    {
        public Int32 x;
        public Int32 y;
        public IntPtr hSource;
        public UInt32 dwID;
        public UInt32 dwFlags;
        public UInt32 dwMask;
        public UInt32 dwTime;
        public Int64 dwExtraInfo;
        public UInt32 cxContact;
        public UInt32 cyContact;
    }

    public interface ITouchEventListener
    {
        void OnTouchEvent(Vector2 position, Vector2 size, UInt32 id, UInt32 flags);
    }

    public class TouchDriver : IDisposable
    {

        private static readonly uint WM_TOUCH = 0x0240;
        
        private IntPtr HWnd;
        private static IntPtr oldproc;
        private bool bound;
        private bool registered_touch;
        private static ITouchEventListener listener;

        delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        private WndProcDelegate procfunc;

        //private static TouchDriver instance;
        //
        //public static TouchDriver GetInstance(ref GlfwNativeWindow wnd)
        //{
        //    if (instance == null)
        //    {
        //        instance = new TouchDriver(ref wnd);
        //    }
        //    return instance;
        //}
        public TouchDriver(ref GlfwNativeWindow wnd)
        {
            GetWindowPtr(ref wnd);
            LinkToWin32();
        }

        public void SetEventListener(ITouchEventListener lst)
        {
            if (lst == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                listener = lst;
            }
        }

        private unsafe void GetWindowPtr(ref GlfwNativeWindow wnd)
        {
            if (wnd.Win32.HasValue)
            {
                HWnd = wnd.Win32.Value.Hwnd;
            }
            else
            {
                throw new Exception("Could not capture native window ptr");
            }
        }

        private unsafe void LinkToWin32()
        {
            registered_touch = RegisterTouchWindow(HWnd, (ulong)RegisterTouchFlags.TWF_WANTPALM);
            if (registered_touch)
            {
                procfunc = new WndProcDelegate(WndProc);
                IntPtr del = Marshal.GetFunctionPointerForDelegate(procfunc);
                oldproc = SetWindowLongPtr(HWnd, -4, del);
                bound = true;
            }
        }

        private static unsafe IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam)
        {
            if (msg == WM_TOUCH)
            {
                long touch_event_count = wparam.ToInt64();
                int touch_arr_size = (int)(sizeof(TOUCH_STRUCT) * touch_event_count);
                IntPtr touch_arr = Marshal.AllocHGlobal(touch_arr_size);
                bool succes = GetTouchInputInfo(lparam, (uint)touch_event_count, touch_arr, (uint)sizeof(TOUCH_STRUCT));
                if (succes)
                {
                    TOUCH_STRUCT* ptr = (TOUCH_STRUCT*)touch_arr.ToPointer();
                    for (int i = 0; i < (int)touch_event_count; i++)
                    {
                        TOUCH_STRUCT ts = ptr[i];
                        Vector2 pos = new Vector2(ts.x, ts.y);
                        Vector2 size = new Vector2(ts.cxContact, ts.cyContact);
                        listener?.OnTouchEvent(pos, size, ts.dwID, ts.dwFlags);
                    }
                    CloseTouchInputHandle(lparam);
                }
            }
            return (IntPtr)CallWindowProc(oldproc, hwnd, msg, wparam, lparam);
        }

        public void Dispose()
        {
            if (registered_touch)
            {
                UnregisterTouchWindow(this.HWnd);
            }
        }

        #region USERDLL
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

        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern long GetLastError();
        #endregion

    }
}
