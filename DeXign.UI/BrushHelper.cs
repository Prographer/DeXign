using System.Collections.Generic;
using System.Windows.Media;

namespace DeXign.UI
{
    public static class BrushHelper
    {
        private static Dictionary<string, Brush> brushes;
        
        static BrushHelper()
        {
            brushes = new Dictionary<string, Brush>();
        }

        public static SolidColorBrush ToBrush(this string htmlColor)
        {
            return FromString(htmlColor);
        }

        public static SolidColorBrush FromString(string htmlColor)
        {
            htmlColor = htmlColor.ToLower();

            if (!brushes.ContainsKey(htmlColor))
                brushes[htmlColor] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(htmlColor));

            return brushes[htmlColor] as SolidColorBrush;
        }
    }
}
