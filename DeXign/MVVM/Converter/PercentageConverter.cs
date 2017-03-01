using System;
using System.Text.RegularExpressions;

namespace DeXign.Converter
{
    class PercentageConverter : BaseValueConverter<double, string>
    {
        public double Maximum { get; set; } = 1;
        public double Minimum { get; set; } = 0;

        public override string Convert(double value, object parameter)
        {
            return $"{Math.Round((value - Minimum) / (Maximum - Minimum) * 100, 0)}%";
        }

        public override double ConvertBack(string value, object parameter)
        {
            if (!Regex.IsMatch(value, @"\d+"))
                throw new Exception();

            int v = int.Parse(Regex.Match(value, @"\d+").Value);

            v = Math.Max(v, 0);
            v = Math.Min(v, 100);

            return v / 100 * (Maximum - Minimum) + Minimum;
        }
    }
}
