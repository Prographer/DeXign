using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeXign.Extension
{
    public static class StringEx
    {
        public static bool IsMatch(this string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

        public static bool AnyEquals(this string input, string obj)
        {
            return input.Equals(obj, StringComparison.OrdinalIgnoreCase);
        }

        public static TEnum? ToEnum<TEnum>(this string input) where TEnum : struct
        {
            var type = typeof(TEnum);

            if (type.IsEnum)
            {
                var q = type.GetEnumValues()
                    .Cast<TEnum>()
                    .Where(e => e.ToString().AnyEquals(input));

                if (q.Count() > 0)
                    return q.First();
            }

            return null;
        }
    }
}
