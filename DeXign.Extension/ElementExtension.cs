using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DeXign.Extension
{
    public static class ElementExtension
    {
        public static bool IsHitted(this FrameworkElement element, MouseEventArgs e)
        {
            return element.IsVisible && VisualTreeHelper.HitTest(element, e.GetPosition(element)) != null;
        }

        public static bool IsBoundHitted(this FrameworkElement element, MouseEventArgs e)
        {
            var position = e.GetPosition(element);

            return
                element.IsVisible &&
                (position.X >= 0) &&
                (position.Y >= 0) &&
                (position.X <= element.ActualWidth) &&
                (position.Y <= element.ActualHeight);
        }
    }
}
