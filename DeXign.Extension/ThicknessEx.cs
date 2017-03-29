using System;
using System.Windows;

namespace DeXign.Extension
{
    public static class ThicknessEx
    {
        public static Thickness Clean(this Thickness thickness, int decimalLength = 0)
        {
            thickness.Left = Math.Round(thickness.Left, decimalLength);
            thickness.Right = Math.Round(thickness.Right, decimalLength);
            thickness.Top = Math.Round(thickness.Top, decimalLength);
            thickness.Bottom = Math.Round(thickness.Bottom, decimalLength);

            return thickness;
        }

        public static double Sum(this Thickness a)
        {
            return a.Left + a.Top + a.Right + a.Bottom;
        }

        public static Thickness Absolute(this Thickness a)
        {
            return new Thickness(
                Math.Abs(a.Left),
                Math.Abs(a.Top),
                Math.Abs(a.Right),
                Math.Abs(a.Bottom));
        }

        public static Thickness Subtract(this Thickness a, Thickness b)
        {
            return new Thickness(
                a.Left - b.Left,
                a.Top - b.Top,
                a.Right - b.Right,
                a.Bottom - b.Bottom);
        }

        public static Thickness Add(this Thickness a, Thickness b)
        {
            return new Thickness(
                a.Left + b.Left,
                a.Top + b.Top,
                a.Right + b.Right,
                a.Bottom + b.Bottom);
        }
    }
}
