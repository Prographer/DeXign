using DeXign.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeXign.Extension;

namespace DeXign.Core.Designer
{
    public static class DesignerManager
    {
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
            return declarType.GetProperties()
                .Where(pi => pi.HasAttribute<DesignElementAttribute>())
                .Select(pi => new AttributeTuple<DesignElementAttribute, PropertyInfo>(
                    pi.GetAttribute<DesignElementAttribute>(), pi));
        }
    }
}
