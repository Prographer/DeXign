using System;
using System.Reflection;
using System.Windows;

namespace DeXign.Core
{
    internal static class ReflectionEx
    {
        public static TAttribute GetAttribute<TAttribute>(this object obj)
             where TAttribute : Attribute
        {
            Type type = obj.GetType();
            TAttribute attr = type.GetCustomAttribute<TAttribute>();

            return attr;
        }

        public static bool HasAttribute<TAttribute>(this object obj)
            where TAttribute : Attribute
        {
            return obj.GetAttribute<TAttribute>() != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo pi)
            where TAttribute : Attribute
        {
            TAttribute attr = pi.GetCustomAttribute<TAttribute>();

            return attr;
        }

        public static bool HasAttribute<TAttribute>(this PropertyInfo pi)
            where TAttribute : Attribute
        {
            return pi.GetAttribute<TAttribute>() != null;
        }

        public static bool CanCastingTo<T>(this PropertyInfo pi)
        {
            return typeof(T).IsAssignableFrom(pi.PropertyType);
        }

        public static DependencyProperty GetDependencyProperty(this PropertyInfo pi)
        {
            // DependencyProperty
            var dpField = pi.DeclaringType.GetField($"{pi.Name}Property");
            
            if (dpField != null)
            {
                return dpField.GetValue(null) as DependencyProperty;
            }

            // DependencyPropertyKey (ReadOnly)
            var dpKeyField = pi.DeclaringType.GetField($"{pi.Name}PropertyKey", BindingFlags.NonPublic);

            if (dpKeyField != null)
            {
                var dpKey = dpKeyField.GetValue(null) as DependencyPropertyKey;

                return dpKey.DependencyProperty;
            }

            return null;
        }

        public static bool IsDefaultDependencyProperty(this PropertyInfo pi, DependencyObject parent)
        {
            var dp = pi.GetDependencyProperty();

            if (dp != null)
            {
                var v = pi.GetValue(parent);
                var v2 = dp.DefaultMetadata.DefaultValue;

                return v.Equals(v2);
            }

            return false;
        }
    }
}