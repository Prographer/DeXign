using Phlet.Extension;
using System.Windows;

namespace Phlet.Core.Controls
{
    [XForms("Xamarin.Forms", "View")]
    public class PControl : PVisual
    {
        public static readonly DependencyProperty VerticalOptionsProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty HorizontalOptionsProperty =
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
            get { return GetValue<LayoutAlignment>(VerticalOptionsProperty); }
            set { SetValue(VerticalOptionsProperty, value); }
        }

        [XForms("HorizontalOptions")]
        public LayoutAlignment HorizontalAlignment
        {
            get { return GetValue<LayoutAlignment>(HorizontalOptionsProperty); }
            set { SetValue(HorizontalOptionsProperty, value); }
        }
    }
}