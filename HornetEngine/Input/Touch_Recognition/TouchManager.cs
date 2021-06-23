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
            this.touchObjects = new List<TouchObject>();

            this.panel = panel;
        }

        /// <summary>
        /// A function which will initialize all the touchpoints.
        /// </summary>
        /// <param name="touchPoints">A list of all the touchpoints</param>
        public void InitializeTouchpoints(List<TouchPoint> touchPoints)
        {
            // TODO Check welke punten bij elkaar horen
            // TODO Initialiseer de lijst touchObjects met de verschillende objecten
            
            //Console.WriteLine("Initialization\nSize: {0}", touchPoints.Count);

            try
            {
                for (int i = 0; i < touchPoints.Count; i += 3)
                {
                    List<TouchPoint> newList = new List<TouchPoint>();
                    newList.Add(touchPoints[i]);
                    newList.Add(touchPoints[i + 1]);
                    newList.Add(touchPoints[i + 2]);

                    TouchObject touchObj = new TouchObject(SizeCheck(newList));
                    touchObjects.Add(touchObj);
                }

                Console.WriteLine("Touch obj length: {0}", touchObjects.Count);
            } catch
            {

            }
        }

        /// <summary>
        /// A function which will check which touchpoint is the largets, and orders the list of touchpoints.
        /// </summary>
        /// <param name="touchPoints">A list of touchpoints, where the first entry is the touchpoint with the largest surface area.</param>
        /// <returns></returns>
        private Vector2[] SizeCheck(List<TouchPoint> touchPoints)
        {
            // Initialize the surface area
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


            //Console.WriteLine("Size 0: {0}\nSize 1: {1}\nSize 2: {2}", touch_vector[0], touch_vector[1], touch_vector[2]);

            return touch_vector;
        }

        /// <summary>
        /// A function which ensures that all the TouchPoints will be refreshed each frame.
        /// </summary>
        public void Refresh()
        {
            List<TouchPoint> touchPoints = panel.GetTouchPoints();
            touchObjects.Clear();
            
            Console.WriteLine("\n\nRefresh called");
            InitializeTouchpoints(touchPoints);
        }
    }
}
