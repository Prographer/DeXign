using DeXign.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeXign.Core.Designer
{
    public static class DesignerManager
    {
        public static IEnumerable<AttributeTuple<DesignElementAttribute, Type>> GetElementTypes()
        {
            return Assembly.GetAssembly(typeof(PObject))
                .GetTypes()
                .Where(t => t.HasAttribute<DesignElementAttribute>())
                .Select(t => new AttributeTuple<DesignElementAttribute, Type>(t.GetAttribute<DesignElementAttribute>(), t));
        }
    }
}
