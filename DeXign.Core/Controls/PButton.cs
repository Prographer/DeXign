using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using WPFExtension;

namespace DeXign.Core.Controls
{
    [ContentProperty("Text")]
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "버튼")]
    [XForms("Xamarin.Forms", "Button")]
    public class PButton : PControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty ForegroundProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyHelper.Register<Brush>();

        [DesignElement(Category = Constants.Property.Design, DisplayName = "텍스트")]
        [XForms("Text")]
        public string Text
        {
            get { return GetValue<string>(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "텍스트 색상")]
        [XForms("TextColor")]
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

        [DesignElement(Category = Constants.Property.Design, DisplayName = "버튼 보더 두께")]
        [XForms("BorderWidth")]
        public double BorderThickness
        {
            get { return GetValue<double>(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "버튼 보더 색상")]
        [XForms("BorderColor")]
        public Brush BorderBrush
        {
            get { return GetValue<Brush>(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public PButton()
        {

        }
    }
}