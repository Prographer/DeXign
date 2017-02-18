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
                new PropertyMetadata(LayoutAlignment.Fill));

        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyHelper.Register(
                new PropertyMetadata(LayoutAlignment.Fill));

        public static readonly DependencyProperty MarginProperty =
            DependencyHelper.Register();

        [DesignElement(Category = Constants.Property.Blank, DisplayName = "바깥 여백")]
        [XForms("Margin")]
        public Thickness Margin
        {
            get { return GetValue<Thickness>(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Layout, DisplayName = "세로 정렬")]
        [XForms("VerticalOptions")]
        public LayoutAlignment VerticalAlignment
        {
            get { return GetValue<LayoutAlignment>(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Layout, DisplayName = "가로 정렬")]
        [XForms("HorizontalOptions")]
        public LayoutAlignment HorizontalAlignment
        {
            get { return GetValue<LayoutAlignment>(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }
    }
}