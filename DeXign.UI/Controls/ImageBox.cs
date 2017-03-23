using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WPFExtension;

namespace DeXign.UI
{
    public class ImageBox : Control
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty StretchProperty =
            DependencyHelper.Register();

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
    }
}
