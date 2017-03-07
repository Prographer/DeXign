using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;

using DeXign.Core.Collections;

using WPFExtension;

namespace DeXign.Core.Controls
{
    public class PLayout : PControl
    {
        public static readonly DependencyProperty PaddingProperty =
            DependencyHelper.Register();

        [DesignElement(Category = Constants.Property.Blank, DisplayName = "안쪽 여백")]
        [XForms("Padding")]
        public Thickness Padding
        {
            get { return GetValue<Thickness>(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
    }

    [ContentProperty("Children")]
    [XForms("Blank", "Xamarin.Forms")]
    public class PLayout<T> : PLayout
        where T : PControl
    {
        [XForms("Children")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PControlCollection<T> Children { get; } = new PControlCollection<T>();
    }
}
