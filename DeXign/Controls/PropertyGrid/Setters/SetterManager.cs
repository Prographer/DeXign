using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;

using DeXign.Extension;

namespace DeXign.Controls
{
    public static class SetterManager
    {
        static Dictionary<Type, Type> setterTypes;
        static Dictionary<string, Type> setterNameTypes;

        static SetterManager()
        {
            setterTypes = new Dictionary<Type, Type>();
            setterNameTypes = new Dictionary<string, Type>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.HasAttribute<SetterAttribute>())
                {
                    var attr = type.GetAttribute<SetterAttribute>();

                    if (string.IsNullOrEmpty(attr.Key))
                        setterTypes[attr.Type] = type;
                    else
                        setterNameTypes[attr.Key] = type;
                }
            }
        }

        public static ISetter CreateSetter(DependencyObject target, PropertyInfo pi)
        {
            Type type = pi.PropertyType;

            if (!setterTypes.ContainsKey(type))
            {
                if (type.IsEnum)
                    type = typeof(Enum);
                else
                    return null;
            }

            return (ISetter)Activator.CreateInstance(
                setterTypes[type],
                new object[] { target, pi });
        }

        public static ISetter CreateSetter(DependencyObject target, PropertyInfo pi, string name)
        {
            if (!setterNameTypes.ContainsKey(name))
                return null;

            return (ISetter)Activator.CreateInstance(
                setterNameTypes[name],
                new object[] { target, pi });
        }
    }
}