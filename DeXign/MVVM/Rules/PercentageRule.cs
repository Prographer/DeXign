using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace DeXign.Rules
{
    public class PercentageRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string sValue = value.ToString();

            return new ValidationResult(Regex.IsMatch(sValue, @"\d+"), null);
        }
    }
}
