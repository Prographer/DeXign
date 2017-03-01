using System.Windows;

namespace DeXign.Extension
{
    public static class RectEx
    {
        public static double CenterX(this Rect rect)
        {
            return rect.Left + rect.Width / 2;
        }

        public static double CenterY(this Rect rect)
        {
            return rect.Top + rect.Height / 2;
        }
    }
}
