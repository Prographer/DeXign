namespace DeXign.Converter
{
    class ReciprocalConverter : BaseValueConverter<double, double>
    {
        public override double Convert(double value, object parameter)
        {
            return 1d / value;
        }

        public override double ConvertBack(double value, object parameter)
        {
            return 1d / value;
        }
    }
}
