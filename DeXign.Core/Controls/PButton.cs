using System;
using System.Windows;
using System.Windows.Media;

using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "버튼")]
    [XForms("Xamarin.Forms", "Button")]
    [WPF("System.Windows.Controls", "Button")]
    public class PButton : PControl
    {
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
        [WPF("Content")]
        public string Text
        {
            get { return GetValue<string>(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Brush, DisplayName = "텍스트 색상")]
        [XForms("TextColor")]
        [WPF("Foreground")]
        public Brush Foreground
        {
            get { return GetValue<Brush>(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "모서리")]
        [XForms("BorderRadius")]
        public double BorderRadius
        {
            get { return GetValue<double>(BorderRadiusProperty); }
            set { SetValue(BorderRadiusProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "버튼 보더 두께", Visible = false)]
        [XForms("BorderWidth")]
        [WPF("BorderThickness")]
        public double BorderThickness
        {
            get { return GetValue<double>(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Brush, DisplayName = "버튼 보더 색상", Visible = false)]
        [XForms("BorderColor")]
        [WPF("BorderBrush")]
        public Brush BorderBrush
        {
            get { return GetValue<Brush>(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "폰트 크기")]
        [XForms("FontSize")]
        [WPF("FontSize")]
        public double FontSize
        {
            get { return GetValue<double>(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public PButton()
        {

        }
    }
}