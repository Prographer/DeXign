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
            get { return this.GetValue<ImageSource>(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public Stretch Stretch
        {
            get { return this.GetValue<Stretch>(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
    }
}
