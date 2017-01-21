using DeXign.Extension;
using System.Windows;

namespace DeXign.Core.Controls
{
    [XForms("Xamarin.Forms", "View")]
    public class PControl : PVisual
    {
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty MarginProperty =
            DependencyHelper.Register();
       
        [XForms("Margin")]
        public Thickness Margin
        {
            get { return GetValue<Thickness>(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        [XForms("VerticalOptions")]
        public LayoutAlignment VerticalAlignment
        {
            get { return GetValue<LayoutAlignment>(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        [XForms("HorizontalOptions")]
        public LayoutAlignment HorizontalAlignment
        {
            get { return GetValue<LayoutAlignment>(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }
    }
}