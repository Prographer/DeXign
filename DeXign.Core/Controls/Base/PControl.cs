using System.Windows;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [XForms("Xamarin.Forms", "View")]
    public class PControl : PVisual
    {
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyHelper.Register(
                new PropertyMetadata(PVerticalAlignment.Stretch));

        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyHelper.Register(
                new PropertyMetadata(PHorizontalAlignment.Stretch));

        public static readonly DependencyProperty MarginProperty =
            DependencyHelper.Register();

        [DesignElement(Category = Constants.Property.Blank, DisplayName = "바깥 여백")]
        [XForms("Margin")]
        [WPF("Margin")]
        public Thickness Margin
        {
            get { return this.GetValue<Thickness>(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Layout, DisplayName = "세로 정렬", IsNotEnum = true)]
        [XForms("VerticalOptions")]
        [WPF("VerticalAlignment")]
        public PVerticalAlignment VerticalAlignment
        {
            get { return this.GetValue<PVerticalAlignment>(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Layout, DisplayName = "가로 정렬", IsNotEnum = true)]
        [XForms("HorizontalOptions")]
        [WPF("HorizontalAlignment")]
        public PHorizontalAlignment HorizontalAlignment
        {
            get { return this.GetValue<PHorizontalAlignment>(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }
    }
}