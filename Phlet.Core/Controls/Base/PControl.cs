using Phlet.Extension;
using System.Windows;

namespace Phlet.Core.Controls
{
    public class PControl : DependencyObject
    {
        public static readonly DependencyProperty NameProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty VerticalOptionsProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty HorizontalOptionsProperty =
            DependencyHelper.Register();

        [XForms("Name")]
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        [XForms("VerticalOptions")]
        public LayoutOptions VerticalOptions
        {
            get { return GetValue<LayoutOptions>(VerticalOptionsProperty); }
            set { SetValue(VerticalOptionsProperty, value); }
        }

        [XForms("HorizontalOptions")]
        public LayoutOptions HorizontalOptions
        {
            get { return GetValue<LayoutOptions>(HorizontalOptionsProperty); }
            set { SetValue(HorizontalOptionsProperty, value); }
        }

        public PControl()
        {
        }

        public T GetValue<T>(DependencyProperty dp)
        {
            return (T)GetValue(dp);
        }
    }
}