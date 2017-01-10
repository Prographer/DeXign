using Phlet.Extension;
using System.Windows;

namespace Phlet.Core.Controls
{
    [XForms("View")]
    public class PControl : PObject
    {
        public static readonly DependencyProperty NameProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty VerticalOptionsProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty HorizontalOptionsProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty MarginProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty SpacingProperty =
            DependencyHelper.Register();

        // for resources
        public string Id { get; set; }

        [XForms("Name")]
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        [XForms("Margin")]
        public Thickness Margin
        {
            get { return GetValue<Thickness>(MarginProperty); }
            set { SetValue(MarginProperty, value); }
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

        [XForms("Spacing")]
        public double Spacing
        {
            get { return GetValue<double>(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }
    }
}