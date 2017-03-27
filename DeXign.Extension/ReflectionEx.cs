using System;
using System.Collections;
using System.Reflection;
using System.Windows;

namespace DeXign.Extension
{
    public static class ReflectionEx
    {
        public class TargetDependencyPropertyAttribute : Attribute
        {
            public string PropertyName { get; set; }
        }

        public static TAttribute GetAttribute<TAttribute>(this object obj)
             where TAttribute : Attribute
        {
            if (obj.GetType().IsEnum)
            {
                FieldInfo fi = obj.GetType().GetField(obj.ToString());
                var attr = fi.GetCustomAttribute<TAttribute>();

                return attr;
            }

            if (obj is PropertyInfo pi)
            {
                return pi.GetAttribute<TAttribute>();
            }

            return obj.GetType().GetAttribute<TAttribute>();
        }

        public static bool HasAttribute<TAttribute>(this object obj)
            where TAttribute : Attribute
        {
            return obj.GetAttribute<TAttribute>() != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this EventInfo ei)
            where TAttribute : Attribute
        {
            TAttribute attr = ei.GetCustomAttribute<TAttribute>();

            return attr;
        }

        public static bool HasAttribute<TAttribute>(this EventInfo ei)
            where TAttribute : Attribute
        {
            return ei.GetAttribute<TAttribute>() != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this MethodInfo mi)
            where TAttribute : Attribute
        {
            TAttribute attr = mi.GetCustomAttribute<TAttribute>();

            return attr;
        }

        public static bool HasAttribute<TAttribute>(this MethodInfo mi)
            where TAttribute : Attribute
        {
            return mi.GetAttribute<TAttribute>() != null;
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

        public static TAttribute GetAttribute<TAttribute>(this ParameterInfo pi)
            where TAttribute : Attribute
        {
            TAttribute attr = pi.GetCustomAttribute<TAttribute>();

            return attr;
        }

        public static bool HasAttribute<TAttribute>(this ParameterInfo pi)
            where TAttribute : Attribute
        {
            return pi.GetAttribute<TAttribute>() != null;
        }

        public static TAttribute GetAttribute<TAttribute>(this Type type)
             where TAttribute : Attribute
        {
            TAttribute attr = type.GetCustomAttribute<TAttribute>();

            return attr;
        }

        public static bool HasAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return type.GetAttribute<TAttribute>() != null;
        }

        public static bool CanCastingTo<T>(this PropertyInfo pi)
        {
            return typeof(T).IsAssignableFrom(pi.PropertyType);
        }

        public static bool CanCastingTo<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static DependencyProperty FindDependencyProperty(this DependencyObject obj, string propertyName)
        {
            return obj
                .GetType()
                .GetProperty(propertyName)?
                .GetDependencyProperty();
        }

        public static DependencyProperty GetDependencyProperty(this PropertyInfo pi)
        {
            string name = $"{pi.Name}Property";

            if (pi.HasAttribute<TargetDependencyPropertyAttribute>())
                name = pi.GetAttribute<TargetDependencyPropertyAttribute>().PropertyName;

            // DependencyProperty
            var dpField = pi.DeclaringType.GetField(name);
            
            if (dpField != null)
                return dpField.GetValue(null) as DependencyProperty;

            // DependencyPropertyKey (ReadOnly)
            var dpKeyField = pi.DeclaringType.GetField($"{pi.Name}PropertyKey", BindingFlags.NonPublic | BindingFlags.Static);

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
                
                if (v != null && pi.CanCastingTo<IList>())
                {
                    var lst = (IList)v;

                    return lst.Count == 0;
                }

                return object.Equals(v, v2);
            }

            return false;
        }

        public static object GetDefault(this Type t)
        {
            Func<object> f = GetDefault<object>;

            return f.Method
                .GetGenericMethodDefinition()
                .MakeGenericMethod(t)
                .Invoke(null, null);
        }

        private static T GetDefault<T>()
        {
            return default(T);
        }
    }
}