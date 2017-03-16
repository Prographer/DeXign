using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeXign.Core
{
    internal static class DesignDescriptionDescriptor
    {
        // #(Param1, Param2, ...)
        // #(Param1, !Param2, ...)

        public static IEnumerable<(string Name, bool Visible)> GetParameterNames(DesignDescriptionAttribute attr)
        {
            string pattern = $@"^#\((.*)\)$";

            Match match = Regex.Match(attr.Description, pattern);

            if (match.Success)
            {
                string paramLine = match.Groups[1].Value;

                return Regex.Split(paramLine, " *, *")
                    .Select(n =>
                    {
                        if (n.StartsWith("!"))
                        {
                            return (Regex.Match(n, $"(?<=!).*").Value, false);
                        }

                        return (n, true);
                    });
            }

            return null;
        }
    }
}
