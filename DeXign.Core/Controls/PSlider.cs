using DeXign.Extension;
using System.Windows;
using WPFExtension;

namespace DeXign.Core.Controls
{
    [DesignElement(Category = Constants.Designer.Control, DisplayName = "슬라이더")]
    [DesignElementIgnore("Background")]
    [XForms("Xamarin.Forms", "Slider")]
    [WPF("System.Windows.Controls", "Slider")]
    public class PSlider : PControl
    {
        public static readonly DependencyProperty MinimumProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty MaximumProperty =
            DependencyHelper.Register(new PropertyMetadata(1d));

        public static readonly DependencyProperty ValueProperty =
            DependencyHelper.Register(new PropertyMetadata(0.5d));

        [DesignElement(Category = Constants.Property.Design, DisplayName = "최소 진행도")]
        [XForms("Minimum")]
        [WPF("Minimum")]
        public double Minimum
        {
            get { return this.GetValue<double>(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "최대 진행도")]
        [XForms("Maximum")]
        [WPF("Maximum")]
        public double Maximum
        {
            get { return this.GetValue<double>(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Design, DisplayName = "진행도")]
        [XForms("Value")]
        [WPF("Value")]
        public double Value
        {
            get { return this.GetValue<double>(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        
        [DesignElement(Key = "Percentage", Category = Constants.Property.Design, DisplayName = "진행도(%)")]
        [ReflectionEx.TargetDependencyProperty(PropertyName = "ValueProperty")]
        public double FakeValue
        {
            get { return Value; }
            set { Value = value; }
        }
    }
}
