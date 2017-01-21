using Phlet.Extension;

using System.Windows;
using System.Windows.Media;

namespace Phlet.Core.Controls
{
    [XForms("Xamarin.Forms", "Label")]
    public class PLabel : PControl, IFontControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty FontAttributesProperty = 
            DependencyHelper.Register(new PropertyMetadata(FontAttributes.None));

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty ForegroundProperty =
            DependencyHelper.Register(new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyHelper.Register(new PropertyMetadata(12d));

        [XForms("Text")]
        public string Text
        {
            get { return GetValue<string>(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        [XForms("VerticalTextAlignment")]
        public TextAlignment VerticalTextAlignment
        {
            get { return GetValue<TextAlignment>(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        [XForms("HorizontalTextAlignment")]
        public TextAlignment HorizontalTextAlignment
        {
            get { return GetValue<TextAlignment>(HorizontalTextAlignmentProperty); }
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

        public FontFamily FontFamily
        {
            get { return GetValue<FontFamily>(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [XForms("TextColor")]
        public Brush Foreground
        {
            get { return GetValue<Brush>(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // TODO: Need binding converter (PFontAttributes -> FontWeights or FontStyles)
        [XForms("FontAttributes")]
        public FontAttributes FontAttributes
        {
            get { return GetValue<FontAttributes>(FontAttributesProperty); }
            set { SetValue(FontAttributesProperty, value); }
        }

        [XForms("FontSize")]
        public double FontSize
        {
            get { return GetValue<double>(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
    }
}