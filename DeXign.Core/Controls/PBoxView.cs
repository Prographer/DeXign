using System.Windows;
using System.Windows.Media;

using DeXign.SDK;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "사각형")]
    [DesignElementIgnore("Background")]
    [DXIgnore("Background")]
    [XForms("Xamarin.Forms", "BoxView")]
    [WPF("System.Windows.Shapes", "Rectangle")]
    public class PBoxView : PControl
    {
        public static readonly DependencyProperty FillProperty =
            DependencyHelper.Register(
                new PropertyMetadata(Brushes.Transparent));

        [DesignElement(Category = Constants.Property.Brush, DisplayName = "배경색")]
        [XForms("Color")]
        [WPF("Fill")]
        public Brush Fill
        {
            get { return GetValue<Brush>(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
    }
}
