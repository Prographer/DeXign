using System.Windows;
using System.Windows.Media;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "텍스트")]
    [XForms("Xamarin.Forms", "Label")]
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
        public string Text
        {
            get { return GetValue<string>(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Layout, DisplayName = "텍스트 세로 정렬")]
        [XForms("VerticalTextAlignment")]
        public PVerticalTextAlignment VerticalTextAlignment
        {
            get { return GetValue<PVerticalTextAlignment>(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Layout, DisplayName = "텍스트 가로 정렬")]
        [XForms("HorizontalTextAlignment")]
        public PHorizontalTextAlignment HorizontalTextAlignment
        {
            get { return GetValue<PHorizontalTextAlignment>(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        [XForms("FontFamily")]
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
            get { return GetValue<FontFamily>(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Brush, DisplayName = "텍스트 색상")]
        [XForms("TextColor")]
        public Brush Foreground
        {
            get { return GetValue<Brush>(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // TODO: Need binding converter (PFontAttributes -> FontWeights or FontStyles)
        [XForms("FontAttributes")]
        public PFontAttributes FontAttributes
        {
            get { return GetValue<PFontAttributes>(FontAttributesProperty); }
            set { SetValue(FontAttributesProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "폰트 크기")]
        [XForms("FontSize")]
        public double FontSize
        {
            get { return GetValue<double>(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
    }
}