using System;
using System.Linq;
using System.Reflection;

namespace DeXign.Core
{
    public sealed class LayoutAlignmentConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            if (value != null)
            {
                var parts = value.Split('.');

                if (parts.Length > 2 || (parts.Length == 2 && parts[0] != "LayoutOptions"))
                    throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(LayoutAlignment)}");

                value = parts[parts.Length - 1];

                switch (value)
                {
                    case "Start": return LayoutAlignment.Start;
                    case "Center": return LayoutAlignment.Center;
                    case "End": return LayoutAlignment.End;
                    case "Fill": return LayoutAlignment.Fill;
                    case "StartAndExpand": return LayoutAlignment.StartAndExpand;
                    case "CenterAndExpand": return LayoutAlignment.CenterAndExpand;
                    case "EndAndExpand": return LayoutAlignment.EndAndExpand;
                    case "FillAndExpand": return LayoutAlignment.FillAndExpand;
                }

                FieldInfo field = typeof(LayoutAlignment).GetFields().FirstOrDefault(fi => fi.IsStatic && fi.Name == value);

                if (field != null)
                    return (LayoutAlignment)field.GetValue(null);
            }

            throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(LayoutAlignment)}");
        }
    }
}
