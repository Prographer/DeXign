using System.Windows;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "이미지")]
    [DesignElementIgnore("Background")]
    [XForms("Xamarin.Forms", "Image")]
    public class PImage : PControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyHelper.Register();

        [DesignElement(Key = "ImageSource", Category = Constants.Property.Design, DisplayName = "이미지")]
        [XForms("Source")]
        public string Source
        {
            get { return GetValue<string>(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}
