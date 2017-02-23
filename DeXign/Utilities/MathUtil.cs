using System;
using System.Windows;

namespace DeXign.Utilities
{
    static class MathUtil
    {
        public static Point IntersectsPoint(Rect rect, Point point)
        {
            //Center Point of Rect
            var center = new Point(
               rect.Left + rect.Width / 2,
               rect.Top + rect.Height / 2);

            //Angle at 4 vertices in a rectangle
            double degree = GetAngle(rect.Width, rect.Height);
            double edgeAngle1 = degree;
            double edgeAngle2 = 180 - degree; // degree is always positive
            double edgeAngle3 = 180 + degree;
            double edgeAngle4 = 360 - degree;

            // Get delta between mouse position to center of rect
            double deltaX = point.X - center.X;
            double deltaY = point.Y - center.Y;

            //One line and horizontal angle
            double deltaDegree = GetAngle(deltaX, deltaY);

            if (deltaDegree == 0)
                return new Point(center.X, center.Y);

            // One-dimensional function
            double fx = 0;
            double fy = 0;

            if (edgeAngle1 >= deltaDegree || edgeAngle4 < deltaDegree)
            {
                fx = rect.Right;
                fy = (deltaY / deltaX) * (fx - center.X) + center.Y;
            }
            else if (edgeAngle1 < deltaDegree && edgeAngle2 >= deltaDegree)
            {
                fy = rect.Bottom;
                fx = (fy - center.Y) * (deltaX / deltaY) + center.X;
            }
            else if (edgeAngle2 < deltaDegree && edgeAngle3 >= deltaDegree)
            {
                fx = rect.Left;
                fy = (deltaY / deltaX) * (fx - center.X) + center.Y;
            }
            else if (edgeAngle3 < deltaDegree && edgeAngle4 >= deltaDegree)
            {
                fy = rect.Top;
                fx = (fy - center.Y) * (deltaX / deltaY) + center.X;
            }
            
            return new Point(fx, fy);
        }

        private static double GetAngle(double deltaX, double deltaY)
        {
            double angle = 0;

            if (deltaX == 0 || deltaY == 0)
            {
                if (deltaY == 0)
                    angle = 270 + Math.Sign(deltaX) * 90;

                if (deltaX == 0)
                    angle = 180 - Math.Sign(deltaY) * 90;
            }
            else
            {
                angle = 180 / Math.PI * Math.Atan(deltaY / deltaX);

                if (deltaX < 0 && deltaY > 0)
                    angle += 180;
                else if (deltaX < 0 && deltaY < 0)
                    angle += 180;
                else if (deltaX > 0 && deltaY < 0)
                    angle += 360;
            }

            return angle;
        }
    }
}
