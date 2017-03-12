using System;
using System.Text.RegularExpressions;

namespace DeXign.Core
{
    public static class StringRule
    {
        const string NamingRule = @"^[A-Z][A-Za-z_]+[A-Za-z0-9_]*$";
        const string NamingRuleSmall = @"^[A-Za-z_]+[A-Za-z0-9_]*$";

        const string NamespaceRule = @"[a-zA-Z][a-zA-Z0-9\._-]*";

        public static bool IsValidName(string name, bool allowSmallcase = false)
        {
            if (allowSmallcase)
                return Regex.IsMatch(name, NamingRuleSmall);
            else
                return Regex.IsMatch(name, NamingRule);
        }

        public static bool IsValidNamespace(string @namespace)
        {
            return Regex.IsMatch(@namespace, NamespaceRule);
        }
    }
}
