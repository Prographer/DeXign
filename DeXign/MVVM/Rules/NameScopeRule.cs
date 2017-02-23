using System.Globalization;
using System.Windows.Controls;

namespace DeXign.Rules
{
    public class NameScopeRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(false, null);
        }
    }
}
