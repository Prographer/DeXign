using DeXign.Interop;
using System;
using System.Windows;
using System.Windows.Interop;

namespace DeXign.Windows
{
    public class FloatingWindow : Window
    {
        public FloatingWindow(UIElement element, double width, double height)
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.ShowActivated = false;
            this.ShowInTaskbar = false;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.IsHitTestVisible = false;
            this.Topmost = true;

            this.Background = null;

            this.Width = width;
            this.Height = height;

            this.Content = element;

            if (double.IsNaN(width))
                this.SizeToContent = SizeToContent.Width;

            if (double.IsNaN(height))
                this.SizeToContent = SizeToContent.Height;

            if (double.IsNaN(width) && double.IsNaN(height))
                this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr handle = new WindowInteropHelper(this).Handle;

            NativeMethods.WS wsEx = UnsafeNativeMethods.GetWindowLong(handle, NativeMethods.GWL.EXSTYLE);

            UnsafeNativeMethods.SetWindowLong(handle, NativeMethods.GWL.EXSTYLE, wsEx | NativeMethods.WS.EX_TRANSPARENT);
        }

        public void SetPosition(Point position)
        {
            this.Left = position.X;
            this.Top = position.Y;
        }
    }
}