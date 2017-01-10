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
        public LayoutOptions VerticalAlignment
        {
            get { return GetValue<LayoutOptions>(VerticalOptionsProperty); }
            set { SetValue(VerticalOptionsProperty, value); }
        }

        [XForms("HorizontalOptions")]
        public LayoutOptions HorizontalAlignment
        {
            get { return GetValue<LayoutOptions>(HorizontalOptionsProperty); }
            set { SetValue(HorizontalOptionsProperty, value); }
        }
    }
}