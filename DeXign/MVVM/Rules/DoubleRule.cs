using DeXign.Extension;
using System.Globalization;
using System.Windows.Controls;

namespace DeXign.Rules
{
    class DoubleRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool result = false;

            if (value is double)
                result = true;

            if (value is string strValue)
                result = strValue.TryToDouble(out double v);

            return new ValidationResult(result, null);
        }
    }
}
