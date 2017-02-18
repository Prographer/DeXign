using System.Text.RegularExpressions;

namespace DeXign.Core
{
    public static class StringRule
    {
        const string NamingRule = @"^[A-Z][A-Za-z_]+[A-Za-z0-9_]*";

        public static bool CheckNamingRule(string name)
        {
            return Regex.IsMatch(name, NamingRule);
        }
    }
}
