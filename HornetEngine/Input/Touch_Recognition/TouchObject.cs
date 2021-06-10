using System;
using System.Numerics;

namespace HornetEngine.Input.Touch_Recognition
{
    public class TouchObject
    {
        public Vector2[] touch_points = new Vector2[3];
        public TouchPointType type;

        /// <summary>
        /// The constructor of a touch object
        /// </summary>
        /// <param name="touchpoints">A list of touchpoints, where the first point will always be the FWD</param>
        public TouchObject(Vector2[] touchpoints)
        {
            this.touch_points = touchpoints;
            InitializeObject();
        }

        /// <summary>
        /// A function which can be used to move the touch object to a new location
        /// </summary>
        /// <param name="newPos">An arraylist of touchpoints of the new location</param>
        public void Move(Vector2[] newPos)
        {
            this.touch_points = newPos;
        }

        /// <summary>
        /// A function which will initialize the touch object
        /// </summary>
        private void InitializeObject()
        {
            // Initialize the points
            Vector2 fwd_point = touch_points[0];
            Vector2 point1 = touch_points[1];
            Vector2 point2 = touch_points[2];

            // Obtain the angle between the points
            double angle = AngleBetween(fwd_point, point1, point2);

            // Assign the type, based on the angle
            CheckType(angle);

            Console.WriteLine("Angle1: {0}\nType: {1}", angle, type);
        }

        /// <summary>
        /// A function which will assign a type to a touch object
        /// </summary>
        /// <param name="angle">The angle between the touch points</param>
        private void CheckType(double angle)
        {
            // Assign the invalid type if the angle does not fit the chart
            if(angle < 48 || angle > 80)
            {
                Console.WriteLine("Invalid object");
                type = TouchPointType.INVALID;
            }

            // Assign the correct type for the angles
            if(angle > 46 && angle < 53.99) 
            {
                type = TouchPointType.TYPE1;
            } else if (angle > 54 && angle < 61.99)
            {
                type = TouchPointType.TYPE2;
            } else if (angle > 62 && angle < 69.99)
            {
                type = TouchPointType.TYPE3;
            } else if (angle > 70 && angle < 77.99)
            {
                type = TouchPointType.TYPE4;
            } else if (angle > 78 && angle < 85.99)
            {
                type = TouchPointType.TYPE5;
            } else
            {
                type = TouchPointType.INVALID;
            }
        }


        /*            /|\
         *           / | \
         *          /  |  \
         *         /   |   \
         *     a  /    f    \  b
         *       / q   |   p \
         *      /______|______\
         */

        /// <summary>
        /// A function which will calculate the angles between the touchpoints
        /// </summary>
        /// <param name="fwd">A vector to the FWD touchpoint which can be used for rotation</param>
        /// <param name="vector1">A vector to one of the side touchpoints</param>
        /// <param name="vector2">A vector to one of the side touchpoints</param>
        /// <returns></returns>
        private double AngleBetween(Vector2 fwd, Vector2 vector1, Vector2 vector2)
        {
            // Initialize the middle vector
            Vector2 middle = (vector1 + vector2) / 2;

            // Calculate the length of the lines 'a', 'b' and 'f'
            double f = Vector2.Distance(middle, fwd);
            double a = Vector2.Distance(vector1, fwd);
            double b = Vector2.Distance(vector2, fwd);

            // Calculate the 2 angles
            double angle_q = Math.Asin((f / a)) * (180 / Math.PI);
            double angle_p = Math.Asin((f / b)) * (180 / Math.PI);

            // Calculate the average for a preciser angle
            double avg_angle = (angle_q + angle_p) / 2;

            return avg_angle;
        }
    }
}
