using System;
using System.Windows;

namespace DeXign.Editor
{
    public enum GuidelineDirection : byte
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public class Guideline
    {
        public bool IsVisible { get; set; }

        public Point Point1 { get; set; }
        public Point Point2 { get; set; }

        public GuidelineDirection Direction { get; set; }

        public Guideline SnappedGuideline { get; set; }

        /// <summary>
        /// 0 = 가로, Infinity = 세로
        /// </summary>
        public double Slope
        {
            get
            {
                return (Point2.Y - Point1.Y) / (Point2.X - Point1.X);
            }
        }
        
        public bool IsVertical
        {
            get
            {
                return Slope != 0;
            }
        }

        public Guideline(Point point1, Point point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
            this.SnappedGuideline = null;
        }

        public static double Distance(Guideline gl1, Guideline gl2)
        {
            if (gl2.Slope != gl1.Slope)
                return -1;

            if (gl2.IsVertical)
                return Math.Abs(gl2.Point1.X - gl1.Point1.X);
            else
                return Math.Abs(gl2.Point1.Y - gl1.Point1.Y);
        }
    }
}
