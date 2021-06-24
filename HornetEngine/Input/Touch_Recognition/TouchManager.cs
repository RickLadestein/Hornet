using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HornetEngine.Input.Touch_Recognition
{
    public class TouchManager
    {
        private List<TouchObject> touchObjects;
        private TouchPanel panel;

        /// <summary>
        /// The constructor of the TouchManager
        /// </summary>
        /// <param name="panel">The touchpanel</param>
        public TouchManager(TouchPanel panel)
        {
            // Initialize the list of TouchObjects
            this.touchObjects = new List<TouchObject>();

            this.panel = panel;
        }

        /// <summary>
        /// A function which will initialize all the touchobjects.
        /// </summary>
        /// <param name="touchPoints">A list of all the touchpoints</param>
        public void InitializeTouchObjects(List<TouchPoint> touchPoints)
        {
            // Loop through the list of current touchpoints with steps of 3
            for (int i = 0; i < touchPoints.Count; i += 3)
            {
                // Initialize the list of touchpoints for this object
                List<TouchPoint> newList = new List<TouchPoint>();
                newList.Add(touchPoints[i]);
                newList.Add(touchPoints[i + 1]);
                newList.Add(touchPoints[i + 2]);

                // Initialize and add the object
                TouchObject touchObj = new TouchObject(SizeCheck(newList));
                touchObjects.Add(touchObj);
            }
        }

        /// <summary>
        /// A function which will check which touchpoint is the largest.
        /// </summary>
        /// <param name="touchPoints">A list of touchpoints in a random order.</param>
        /// <returns>A list of touchpoints, where the first entry is the touchpoint with the largest surface area.</returns>
        private Vector2[] SizeCheck(List<TouchPoint> touchPoints)
        {
            // Initialize the surface areas
            uint size_0 = touchPoints[0].contact_height * touchPoints[0].contact_width;
            uint size_1 = touchPoints[1].contact_height * touchPoints[1].contact_width;
            uint size_2 = touchPoints[2].contact_height * touchPoints[2].contact_width;

            Vector2[] touch_vector = new Vector2[3];
           
            // Order the list based on size
            if (size_0 > size_1)
            {
                if (size_0 > size_2)
                {
                    touch_vector[0] = new Vector2(touchPoints[0].xpos, touchPoints[0].ypos);
                    touch_vector[1] = new Vector2(touchPoints[1].xpos, touchPoints[1].ypos);
                    touch_vector[2] = new Vector2(touchPoints[2].xpos, touchPoints[2].ypos);
                }
                else
                {
                    touch_vector[0] = new Vector2(touchPoints[2].xpos, touchPoints[2].ypos);
                    touch_vector[1] = new Vector2(touchPoints[1].xpos, touchPoints[1].ypos);
                    touch_vector[2] = new Vector2(touchPoints[0].xpos, touchPoints[0].ypos);
                }
            }
            else
            {
                if (size_1 > size_2)
                {
                    touch_vector[0] = new Vector2(touchPoints[1].xpos, touchPoints[1].ypos);
                    touch_vector[1] = new Vector2(touchPoints[0].xpos, touchPoints[0].ypos);
                    touch_vector[2] = new Vector2(touchPoints[2].xpos, touchPoints[2].ypos);
                }
                else
                {
                    touch_vector[0] = new Vector2(touchPoints[2].xpos, touchPoints[2].ypos);
                    touch_vector[1] = new Vector2(touchPoints[1].xpos, touchPoints[1].ypos);
                    touch_vector[2] = new Vector2(touchPoints[0].xpos, touchPoints[0].ypos);
                }
            }

            return touch_vector;
        }

        /// <summary>
        /// A function which ensures that all the TouchPoints will be refreshed each frame.
        /// </summary>
        public void Refresh()
        {
            // Get the current touchpoints
            List<TouchPoint> touchPoints = panel.GetTouchPoints();

            // Clear the old list
            touchObjects.Clear();
            
            // Initialize the new touch objects
            InitializeTouchObjects(touchPoints);
        }
    }
}
