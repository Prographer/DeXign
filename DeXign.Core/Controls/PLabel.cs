using System.Windows;
using System.Windows.Media;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "텍스트")]
    [XForms("Xamarin.Forms", "Label")]
    [WPF("clr-namespace:DeXign.UI;assembly=DeXign.UI", "DeXignLabel")]
    public class PLabel : PControl, IFontControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyHelper.Register(new PropertyMetadata(PVerticalTextAlignment.Top));

        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyHelper.Register(new PropertyMetadata(PHorizontalTextAlignment.Left));

        public static readonly DependencyProperty FontAttributesProperty = 
            DependencyHelper.Register(new PropertyMetadata(PFontAttributes.None));

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty ForegroundProperty =
            DependencyHelper.Register(new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyHelper.Register(new PropertyMetadata(12d));

        [DesignElement(Category = Constants.Property.Design, DisplayName = "텍스트")]
        [XForms("Text")]
        [WPF("Text")]
        public string Text
        {
            get { return this.GetValue<string>(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Layout, DisplayName = "텍스트 세로 정렬")]
        [XForms("VerticalTextAlignment")]
        [WPF("VerticalContentAlignment")]
        public PVerticalTextAlignment VerticalTextAlignment
        {
            get { return this.GetValue<PVerticalTextAlignment>(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Layout, DisplayName = "텍스트 가로 정렬")]
        [XForms("HorizontalTextAlignment")]
        [WPF("HorizontalContentAlignment")]
        public PHorizontalTextAlignment HorizontalTextAlignment
        {
            get { return this.GetValue<PHorizontalTextAlignment>(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        [XForms("FontFamily")]
        [WPF("FontFamily")]
        string IFontControl.FontFamily
        {
            get
            {
                return FontFamily?.Source;
            }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "폰트")]
        public FontFamily FontFamily
        {
            get { return this.GetValue<FontFamily>(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Brush, DisplayName = "텍스트 색상")]
        [XForms("TextColor")]
        [WPF("Foreground")]
        public Brush Foreground
        {
            get { return this.GetValue<Brush>(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // TODO: Need binding converter (PFontAttributes -> FontWeights or FontStyles)
        [XForms("FontAttributes")]
        [WPF("FontWeight")]
        public PFontAttributes FontAttributes
        {
            get { return this.GetValue<PFontAttributes>(FontAttributesProperty); }
            set { SetValue(FontAttributesProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "폰트 크기")]
        [XForms("FontSize")]
        [WPF("FontSize")]
        public double FontSize
        {
            get { return this.GetValue<double>(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
    }
}