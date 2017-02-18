using System;
using System.Windows;

namespace DeXign.Extension
{
    public static class ThicknessEx
    {
        public static Thickness Clean(this Thickness thickness, int decimalLength = 2)
        {
            thickness.Left = Math.Round(thickness.Left, decimalLength);
            thickness.Right = Math.Round(thickness.Right, decimalLength);
            thickness.Top = Math.Round(thickness.Top, decimalLength);
            thickness.Bottom = Math.Round(thickness.Bottom, decimalLength);

            return thickness;
        }
    }
}
