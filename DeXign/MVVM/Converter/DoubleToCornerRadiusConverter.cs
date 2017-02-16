using System.Windows;

namespace DeXign.Converter
{
    class DoubleToCornerRadiusConverter : BaseValueConverter<double, CornerRadius>
    {
        public override CornerRadius Convert(double value, object parameter)
        {
            return new CornerRadius(value, value, value, value);
        }

        public override double ConvertBack(CornerRadius value, object parameter)
        {
            return value.TopLeft;
        }
    }
}
