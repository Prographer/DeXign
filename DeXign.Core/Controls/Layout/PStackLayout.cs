using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WPFExtension;

namespace DeXign.Core.Controls
{
    [ContentProperty("Children")]
    [DesignElement(Category = Constants.Designer.Layout, DisplayName = "스택")]
    [XForms("Xamarin.Forms", "StackLayout")]
    [WPF("clr-namespace:DeXign.UI;assembly=DeXign.UI", "SpacingStackPanel")]
    public class PStackLayout : PLayout<PControl>
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyHelper.Register(
                new PropertyMetadata(POrientation.Vertical));

        public static readonly DependencyProperty SpacingProperty =
            DependencyHelper.Register(
                new PropertyMetadata(6d));

        [DesignElement(Category = Constants.Property.Design, DisplayName = "쌓기 방향")]
        [XForms("Orientation")]
        [WPF("Orientation")]
        public POrientation Orientation
        {
            get { return this.GetValue<POrientation>(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Blank, DisplayName = "아이템 사이 공백")]
        [XForms("Spacing")]
        [WPF("Spacing")]
        public double Spacing
        {
            get { return this.GetValue<double>(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public PStackLayout()
        {
        }
    }
}
