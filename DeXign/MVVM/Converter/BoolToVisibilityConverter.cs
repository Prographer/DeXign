using System.Windows;

namespace DeXign.Converter
{
    class BoolToVisibilityConverter : BaseValueConverter<bool, Visibility>
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public override Visibility Convert(bool value, object parameter)
        {
            return value ? TrueValue : FalseValue;
        }

        public override bool ConvertBack(Visibility value, object parameter)
        {
            return value == TrueValue;
        }
    }
}
