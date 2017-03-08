using System;
using System.Text.RegularExpressions;

namespace DeXign.Core
{
    public static class StringRule
    {
        const string NamingRule = @"^[A-Z][A-Za-z_]+[A-Za-z0-9_]*";
        const string NamingRuleSmall = @"^[A-Za-z_]+[A-Za-z0-9_]*";

        const string NamespaceRule = @"^[a-zA-Z]+(?:\.[a-zA-Z])+$";

        public static bool CheckNamingRule(string name, bool allowSmallcase = false)
        {
            if (allowSmallcase)
                return Regex.IsMatch(name, NamingRuleSmall);
            else
                return Regex.IsMatch(name, NamingRule);
        }

        public static bool CheckNamespaceRule(string @namespace)
        {
            return Regex.IsMatch(@namespace, NamespaceRule);
        }
    }
}
