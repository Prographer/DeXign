using System;
using System.Windows;

namespace DeXign.Converter
{
    public class RadiusConverter : BaseValueConverter<FrameworkElement, double>
    {
        public override double Convert(FrameworkElement value, object parameter)
        {
            return Math.Min(value.ActualWidth, value.ActualHeight) / 2f;
        }

        public override FrameworkElement ConvertBack(double value, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}