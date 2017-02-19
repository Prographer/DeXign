using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using DeXign.Extension;
using System.Windows;

namespace DeXign.Controls
{
    public static class SetterManager
    {
        // < PropertyType, SetterType >
        static Dictionary<Type, Type> setterTypes;

        static SetterManager()
        {
            setterTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.HasAttribute<SetterAttribute>())
                .ToDictionary(
                    t => t.GetAttribute<SetterAttribute>().Type,
                    t => t);
        }

        public static ISetter CreateSetter(DependencyObject target, PropertyInfo pi)
        {
            Type type = pi.PropertyType;
            
            if (type.IsEnum)
                type = typeof(Enum);

            if (!setterTypes.ContainsKey(type))
                return null;

            return (ISetter)Activator.CreateInstance(
                setterTypes[type],
                new object[] { target, pi });
        }
    }
}