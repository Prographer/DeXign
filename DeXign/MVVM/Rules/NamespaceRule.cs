using DeXign.Core;
using System.Globalization;
using System.Windows.Controls;

namespace DeXign.Rules
{
    public class NamespaceRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(
                StringRule.CheckNamespaceRule((string)value ?? ""), null);
        }
    }
}
