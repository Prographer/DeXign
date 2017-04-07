using DeXign.Core.Controls;

namespace DeXign.Converter
{
    // PercentageSetter only
    public class SliderValueConverter : BaseValueConverter<double, double>
    {
        public PSlider Slider { get; }

        public SliderValueConverter(PSlider slider)
        {
            this.Slider = slider;
        }

        public override double Convert(double value, object parameter)
        {
            return (value - Slider.Minimum) / (Slider.Maximum - Slider.Minimum);
        }

        public override double ConvertBack(double value, object parameter)
        {
            return Slider.Minimum + (Slider.Maximum - Slider.Minimum) * value;
        }
    }
}
