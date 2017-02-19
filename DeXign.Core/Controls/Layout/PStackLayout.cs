using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WPFExtension;

namespace DeXign.Core.Controls
{
    [ContentProperty("Children")]
    [DesignElement(Category = Constants.Designer.Layout, DisplayName = "스택")]
    [XForms("Xamarin.Forms", "StackLayout")]
    public class PStackLayout : PLayout<PControl>
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyHelper.Register(
                new PropertyMetadata(POrientation.Vertical));

        [DesignElement(Category = Constants.Property.Design, DisplayName = "쌓기 방향")]
        [XForms("Orientation")]
        public POrientation Orientation
        {
            get { return GetValue<POrientation>(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
    }
}
