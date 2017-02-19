using System;
using System.Linq;
using System.ComponentModel;

namespace DeXign.Extension
{
    public static class EnumEx
    {
        public static string GetDescription(this Enum value)
        {
            var attr = value.GetAttribute<DescriptionAttribute>();

            return attr != null ? attr.Description : "";
        }

        public static Enum FromDescription(this string description, Type enumType)
        {
            return Enum.GetValues(enumType)
                .Cast<Enum>()
                .FirstOrDefault(e => e.GetDescription() == description);
        }
    }
}
