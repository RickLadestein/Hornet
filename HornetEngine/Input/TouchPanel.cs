using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using HornetEngine.Util.Drivers;
using HornetEngine.Graphics;
using System.Threading;

namespace HornetEngine.Input
{
    /// <summary>
    /// Structure containing data about a TouchPoint on the screen
    /// </summary>
    public struct TouchPoint
    {
        /// <summary>
        /// The TouchPoint X position
        /// </summary>
        public Int32 xpos;

        /// <summary>
        /// The TouchPoint Y position
        /// </summary>
        public Int32 ypos;

        /// <summary>
        /// The TouchPoint contact area width in pixels
        /// </summary>
        public UInt32 contact_width;

        /// <summary>
        /// The TouchPoint contact area height in pixel
        /// </summary>
        public UInt32 contact_height;

        /// <summary>
        /// The TouchPoint virtual id
        /// </summary>
        public UInt32 virt_id;

    }

    /// <summary>
    /// Class that defines behaviour and events for catching and passing TouchEvents
    /// </summary>
    public class TouchPanel : ITouchEventListener
    {
        public delegate void TouchPointMoveFunc(Vector2 position, Vector2 delta, Vector2 size, uint id);
        public delegate void TouchPointPressFunc(Vector2 position, Vector2 size, uint id);
        public delegate void TouchPointReleaseFunc(Vector2 position, Vector2 size, uint id);

        private Mutex touch_mutex;
        private Dictionary<UInt32, TouchPoint> captured_points;

        /// <summary>
        /// Event queue for listening to Touch Move events
        /// </summary>
        public event TouchPointMoveFunc TouchMove;

        /// <summary>
        /// Event queue for listening to Touch Press events
        /// </summary>
        public event TouchPointPressFunc TouchPress;

        /// <summary>
        /// Event queue for listening to Touch Release events
        /// </summary>
        public event TouchPointReleaseFunc TouchRelease;

        /// <summary>
        /// Instantiates a new instance of TouchPanel that hooks into the touch driver event bus
        /// </summary>
        /// <param name="drv">The TouchDriver instance</param>
        public TouchPanel(TouchDriver drv)
        {
            touch_mutex = new Mutex();
            captured_points = new Dictionary<uint, TouchPoint>();
            captured_points.EnsureCapacity(32);
            drv.SetEventListener(this);
        }

        /// <summary>
        /// Gets a list of all the currently active touch points on the screen
        /// </summary>
        /// <returns>A list of all the active touch points on the screen</returns>
        public List<TouchPoint> GetTouchPoints()
        {
            try
            {
                touch_mutex.WaitOne();
                List<TouchPoint> tps = new List<TouchPoint>(captured_points.Values);
                return tps;
            } catch(Exception ex)
            {
                throw ex;
            } finally
            {
                touch_mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Internal interface function for catching touch events from the touch driver
        /// </summary>
        /// <param name="position">The position of the touch point</param>
        /// <param name="size">The size of the touch point in pixels</param>
        /// <param name="id">The id of the touch point</param>
        /// <param name="flags">The special flags that identifies the event type</param>
        public void OnTouchEvent(Vector2 position, Vector2 size, uint id, uint flags)
        {
            //Check if the flag for touchevent up is true
            if((flags & (uint)TouchEventFlags.TOUCHEVENTF_UP) > 0)
            {
                touch_mutex.WaitOne();
                captured_points.Remove(id);
                touch_mutex.ReleaseMutex();
                return;
            }

            //Check if the flag for touchevent down is true
            if ((flags & (uint)TouchEventFlags.TOUCHEVENTF_DOWN) > 0)
            {
                if(!captured_points.ContainsKey(id))
                {
                    TouchPoint tp = new TouchPoint()
                    {
                        xpos = (int)position.X,
                        ypos = (int)position.Y,
                        contact_width = (uint)size.X,
                        contact_height = (uint)size.Y,
                        virt_id = id
                    };
                    touch_mutex.WaitOne();
                    captured_points.Add(id, tp);
                    touch_mutex.ReleaseMutex();
                }
                TouchPress?.Invoke(position, size, id);
                return;
            }

            //Check if the flag for touchevent move is true
            if ((flags & (uint)TouchEventFlags.TOUCHEVENTF_MOVE) > 0)
            {
                if(captured_points.ContainsKey(id))
                {
                    int delta_x, delta_y;
                    TouchPoint tp = captured_points[id];

                    delta_x = ((int)position.X) - tp.xpos;
                    delta_y = ((int)position.Y) - tp.ypos;


                    tp.xpos = (int)position.X;
                    tp.ypos = (int)position.Y;
                    tp.contact_width = (uint)size.X;
                    tp.contact_height = (uint)size.Y;
                    tp.virt_id = id;
                    captured_points[id] = tp;
                    TouchMove?.Invoke(position, new Vector2(delta_x, delta_y), size, id);
                }
                return;
            }
        }
    }
}
