namespace DeXign.Converter
{
    public class MultiplayConverter : BaseValueConverter<double, double>
    {
        public double Value { get; set; } = 1;

        public override double Convert(double value, object parameter)
        {
            return value * this.Value;
        }

        public override double ConvertBack(double value, object parameter)
        {
            return value / this.Value;
        }
    }
}
