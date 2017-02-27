using System.Windows;

using DeXign.Interop;

namespace DeXign.OS
{
    public static class SystemMouse
    {
        public static Point Position
        {
            get
            {
                NativeMethods.Point point;
                UnsafeNativeMethods.GetCursorPos(out point);
                
                return new Point(point.X, point.Y);
            }
        }
    }
}
