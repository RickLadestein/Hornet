using System;
using System.Numerics;

namespace HornetEngine.Input.Touch_Recognition
{
    public class TouchObject
    {
        public Vector2[] touch_points = new Vector2[3];
        public TouchPointType type;

        public TouchObject(Vector2[] touchpoints)
        {
            this.touch_points = touchpoints;
            InitializeObject();
        }

        public void Move(Vector2[] newPos)
        {
            this.touch_points = newPos;
        }

        private void InitializeObject()
        {
            Vector2 fwd_point = touch_points[0];
            Vector2 point1 = touch_points[1];
            Vector2 point2 = touch_points[2];

            double angle = AngleBetween(fwd_point, point1, point2);

            CheckType(angle);

            Console.WriteLine("Angle1: {0}\nType: {1}", angle, type);
        }

        private void CheckType(double angle)
        {
            if(angle < 48 || angle > 80)
            {
                Console.WriteLine("Invalid object");
                type = TouchPointType.INVALID;
            }

            if(angle > 48 && angle < 52)
            {
                type = TouchPointType.TYPE1;
            } else if (angle > 55 && angle < 59) {
                type = TouchPointType.TYPE2;
            } else if (angle > 62 && angle < 66)
            {
                type = TouchPointType.TYPE3;
            } else if (angle > 69 && angle < 73)
            {
                type = TouchPointType.TYPE4;
            } else if (angle > 76 && angle < 80)
            {
                type = TouchPointType.TYPE5;
            }
        }


        /*            /|\
         *           / | \
         *          /  |  \
         *         /   |   \
         *     a  /  f |    \  b
         *       /     |     \
         *      /______|______\
         */
        private double AngleBetween(Vector2 fwd, Vector2 vector1, Vector2 vector2)
        {
            Vector2 middle = (vector1 + vector2) / 2;

            double f = Vector2.Distance(middle, fwd);
            double a = Vector2.Distance(vector1, fwd);
            double b = Vector2.Distance(vector2, fwd);

            return Math.Asin((f/a)) * (180 / Math.PI);
        }
    }
}
