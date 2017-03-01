using System.Windows;
using System.Windows.Media;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "사각형")]
    [DesignElementIgnore("Background")]
    [XForms("Xamarin.Forms", "BoxView")]
    public class PBoxView : PControl
    {
        public static readonly DependencyProperty FillProperty =
            DependencyHelper.Register(
                new PropertyMetadata(Brushes.Transparent));

        [DesignElement(Category = Constants.Property.Brush, DisplayName = "배경색")]
        [XForms("Color")]
        public Brush Fill
        {
            get { return GetValue<Brush>(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
    }
}
