using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DeXign.Core.Controls;
using DeXign.Extension;

namespace DeXign.Core.Designer
{
    public static class DesignerManager
    {
        static Dictionary<Type, AttributeTuple<DesignElementAttribute, PropertyInfo>[]> props;

        static DesignerManager()
        {
            props = Assembly.GetAssembly(typeof(PObject))
                .GetTypes()
                .Where(t => t.HasAttribute<DesignElementAttribute>())
                .ToDictionary(
                    t => t,
                    t => GetPropertiesCore(t)
                        .Where(p => p.Attribute.Visible).ToArray());
        }

        public static IEnumerable<AttributeTuple<DesignElementAttribute, Type>> GetElementTypes()
        {
            return Assembly.GetAssembly(typeof(PObject))
                .GetTypes()
                .Where(t => t.HasAttribute<DesignElementAttribute>())
                .Select(t => new AttributeTuple<DesignElementAttribute, Type>(
                    t.GetAttribute<DesignElementAttribute>(), t));
        }
        
        public static IEnumerable<AttributeTuple<DesignElementAttribute, PropertyInfo>> GetProperties(Type declarType)
        {
            if (props.ContainsKey(declarType))
                return props[declarType];

            return GetPropertiesCore(declarType);
        }

        private static IEnumerable<AttributeTuple<DesignElementAttribute, PropertyInfo>> GetPropertiesCore(Type declarType)
        {
            return declarType.GetProperties()
                .Where(pi => pi.HasAttribute<DesignElementAttribute>())
                .Select(pi => new AttributeTuple<DesignElementAttribute, PropertyInfo>(
                    pi.GetAttribute<DesignElementAttribute>(), pi));
        }
    }
}
