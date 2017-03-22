using DeXign.SDK;
using System.Windows;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "이미지")]
    [DesignElementIgnore("Background")]
    [DXIgnore("Background")]
    [XForms("Xamarin.Forms", "Image")]
    [WPF("clr-namespace:DeXign.UI;assembly=DeXign.UI", "ImageBox")]
    public class PImage : PControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty StretchProperty =
            DependencyHelper.Register(new PropertyMetadata(PStretch.Uniform));
        
        [DesignElement(Key = "ImageSource", Category = Constants.Property.Design, DisplayName = "이미지")]
        [XForms("Source")]
        [WPF("Source")]
        [DXResource(ResourceType.Image)]
        public string Source
        {
            get { return GetValue<string>(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "이미지 모드")]
        [XForms("Aspect")]
        [WPF("Stretch")]
        public PStretch Stretch
        {
            get { return GetValue<PStretch>(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public PImage()
        {
        }
    }
}
