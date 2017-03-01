namespace DeXign.Converter
{
    class ReciprocalConverter : BaseValueConverter<double, double>
    {
        public double Factor { get; set; } = 1d;

        public override double Convert(double value, object parameter)
        {
            return 1d / value * this.Factor;
        }

        public override double ConvertBack(double value, object parameter)
        {
            return 1d / value * this.Factor;
        }
    }
}
