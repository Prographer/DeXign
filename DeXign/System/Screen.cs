using WPFPoint = System.Windows.Point;
using WPFSize = System.Windows.Size;
using WPFRect = System.Windows.Rect;

using WinScreen = System.Windows.Forms.Screen;
using WinPoint = System.Drawing.Point;

namespace DeXign.OS
{
    public static class SystemScreen
    {
        public static WPFRect GetBounds(WPFPoint p)
        {
            var rect = WinScreen.GetBounds(
                new WinPoint(
                    (int)p.X, 
                    (int)p.Y));

            return new WPFRect(
                new WPFPoint(rect.X, rect.Y),
                new WPFSize(rect.Width, rect.Height));
        }
    }
}
