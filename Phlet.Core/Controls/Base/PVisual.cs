using Phlet.Extension;
using System.Windows;
using System.Windows.Media;

namespace Phlet.Core.Controls
{
    public class PVisual : PObject
    {
        public static readonly DependencyProperty AnchorXProperty
            = DependencyHelper.Register(new PropertyMetadata(0.5d));

        public static readonly DependencyProperty AnchorYProperty
            = DependencyHelper.Register(new PropertyMetadata(0.5d));
        
        public static readonly DependencyProperty BackgroundProperty
            = DependencyHelper.Register(new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty WidthProperty
            = DependencyHelper.Register(new PropertyMetadata(-1d));

        public static readonly DependencyProperty HeightProperty
            = DependencyHelper.Register(new PropertyMetadata(-1d));

        public static readonly DependencyProperty MinWidthProperty
            = DependencyHelper.Register(new PropertyMetadata(-1d));

        public static readonly DependencyProperty MinHeightProperty
            = DependencyHelper.Register(new PropertyMetadata(-1d));

        public static readonly DependencyProperty OpacityProperty
            = DependencyHelper.Register(new PropertyMetadata(1.0d));

        public static readonly DependencyProperty RotationProperty
            = DependencyHelper.Register();

        public static readonly DependencyProperty RotationXProperty
            = DependencyHelper.Register();

        public static readonly DependencyProperty RotationYProperty
            = DependencyHelper.Register();

        public static readonly DependencyProperty XProperty
            = DependencyHelper.Register();

        public static readonly DependencyProperty YProperty
            = DependencyHelper.Register();

        [XForms("AnchorX")]
        public double AnchorX
        {
            get { return GetValue<double>(AnchorXProperty); }
            set { SetValue(AnchorXProperty, value); }
        }
        
        [XForms("AnchorY")]
        public double AnchorY
        {
            get { return GetValue<double>(AnchorYProperty); }
            set { SetValue(AnchorYProperty, value); }
        }

        [XForms("BackgroundColor")]
        public SolidColorBrush Background
        {
            get { return GetValue<SolidColorBrush>(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        [XForms("WidthRequest")]
        public double Width
        {
            get { return GetValue<double>(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        [XForms("HeightRequest")]
        public double Height
        {
            get { return GetValue<double>(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        [XForms("MinimumHeightRequest")]
        public double MinHeight
        {
            get { return GetValue<double>(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        [XForms("MinimumWidthRequest")]
        public double MinWidth
        {
            get { return GetValue<double>(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        [XForms("Opacity")]
        public double Opacity
        {
            get { return GetValue<double>(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        [XForms("Rotation")]
        public double Rotation
        {
            get { return GetValue<double>(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        [XForms("RotationX")]
        public double RotationX
        {
            get { return GetValue<double>(RotationXProperty); }
            set { SetValue(RotationXProperty, value); }
        }

        [XForms("RotationY")]
        public double RotationY
        {
            get { return GetValue<double>(RotationYProperty); }
            set { SetValue(RotationYProperty, value); }
        }
    }
}
