using System;
using System.Windows;
using System.Windows.Media;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "버튼")]
    [XForms("Xamarin.Forms", "Button")]
    [WPF("clr-namespace:DeXign.UI;assembly=DeXign.UI", "DeXignButton")]
    public class PButton : PControl
    {
        [WPF("Click")]
        [DesignElement(Category = Constants.Event.Gesture, DisplayName = "클릭했을 때")]
        [DesignDescription("#(발생자, !e)")]
        public event EventHandler Clicked;

        public static readonly DependencyProperty TextProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty ForegroundProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyHelper.Register(
                new PropertyMetadata(5d));

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyHelper.Register<Brush>();

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

        [DesignElement(Category = Constants.Property.Brush, DisplayName = "텍스트 색상")]
        [XForms("TextColor")]
        [WPF("Foreground")]
        public Brush Foreground
        {
            get { return this.GetValue<Brush>(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "모서리")]
        [XForms("BorderRadius")]
        public double BorderRadius
        {
            get { return this.GetValue<double>(BorderRadiusProperty); }
            set { SetValue(BorderRadiusProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "버튼 보더 두께", Visible = false)]
        [XForms("BorderWidth")]
        [WPF("BorderThickness")]
        public double BorderThickness
        {
            get { return this.GetValue<double>(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Brush, DisplayName = "버튼 보더 색상", Visible = false)]
        [XForms("BorderColor")]
        [WPF("BorderBrush")]
        public Brush BorderBrush
        {
            get { return this.GetValue<Brush>(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "폰트 크기")]
        [XForms("FontSize")]
        [WPF("FontSize")]
        public double FontSize
        {
            get { return this.GetValue<double>(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public PButton()
        {
        }
    }
}