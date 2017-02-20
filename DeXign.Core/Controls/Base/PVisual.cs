using System.Windows;
using System.Windows.Media;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Core.Controls
{
    public class PVisual : PObject
    {
        public static readonly DependencyProperty AnchorXProperty =
            DependencyHelper.Register(new PropertyMetadata(0.5d));

        public static readonly DependencyProperty AnchorYProperty =
            DependencyHelper.Register(new PropertyMetadata(0.5d));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyHelper.Register(new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty WidthProperty =
            DependencyHelper.Register(new PropertyMetadata(-1d));

        public static readonly DependencyProperty HeightProperty =
            DependencyHelper.Register(new PropertyMetadata(-1d));

        public static readonly DependencyProperty MinWidthProperty =
            DependencyHelper.Register(new PropertyMetadata(0d));

        public static readonly DependencyProperty MinHeightProperty =
            DependencyHelper.Register(new PropertyMetadata(0d));

        public static readonly DependencyProperty OpacityProperty =
            DependencyHelper.Register(new PropertyMetadata(1.0d));

        public static readonly DependencyProperty RotationProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty RotationXProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty RotationYProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty XProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty YProperty =
            DependencyHelper.Register();

        [DesignElement(Category = Constants.Property.Transform, DisplayName = "기준점 X")]
        [XForms("AnchorX")]
        public double AnchorX
        {
            get { return GetValue<double>(AnchorXProperty); }
            set { SetValue(AnchorXProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Transform, DisplayName = "기준점 Y")]
        [XForms("AnchorY")]
        public double AnchorY
        {
            get { return GetValue<double>(AnchorYProperty); }
            set { SetValue(AnchorYProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "배경색")]
        [XForms("BackgroundColor")]
        public SolidColorBrush Background
        {
            get { return GetValue<SolidColorBrush>(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "가로 크기")]
        [XForms("WidthRequest")]
        public double Width
        {
            get { return GetValue<double>(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "세로 크기")]
        [XForms("HeightRequest")]
        public double Height
        {
            get { return GetValue<double>(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "세로 최소 크기")]
        [XForms("MinimumHeightRequest")]
        public double MinHeight
        {
            get { return GetValue<double>(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "가로 최소 크기")]
        [XForms("MinimumWidthRequest")]
        public double MinWidth
        {
            get { return GetValue<double>(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        [DesignElement(Key = "Opacity", Category = Constants.Property.Design, DisplayName = "투명도")]
        [XForms("Opacity")]
        public double Opacity
        {
            get { return GetValue<double>(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Transform, DisplayName = "회전")]
        [XForms("Rotation")]
        public double Rotation
        {
            get { return GetValue<double>(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Transform, DisplayName = "3차원 가로 회전")]
        [XForms("RotationX")]
        public double RotationX
        {
            get { return GetValue<double>(RotationXProperty); }
            set { SetValue(RotationXProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Transform, DisplayName = "3차원 세로 회전")]
        [XForms("RotationY")]
        public double RotationY
        {
            get { return GetValue<double>(RotationYProperty); }
            set { SetValue(RotationYProperty, value); }
        }

        [XForms("X")]
        public double X
        {
            get { return GetValue<double>(XProperty); }
            set { SetValue(XProperty, value); }
        }

        [XForms("Y")]
        public double Y
        {
            get { return GetValue<double>(YProperty); }
            set { SetValue(YProperty, value); }
        }
    }
}