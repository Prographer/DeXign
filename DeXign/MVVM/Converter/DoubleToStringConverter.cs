using DeXign.Extension;

namespace DeXign.Converter
{
    class DoubleToStringConverter : BaseValueConverter<double, string>
    {
        public override double ConvertBack(string value, object parameter)
        {
            return value.ToDouble();
        }

        public override string Convert(double value, object parameter)
        {
            if (double.IsNaN(value))
                return "Auto";

            return value.ToString();
        }
    }
}
