using System;
using System.Reflection;

namespace DeXign.Core
{
    public static class ReflectionEx
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
    }
}
