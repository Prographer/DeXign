using DeXign.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeXign.Core.Designer
{
    public static class DesignerManager
    {
        public static IEnumerable<AttributeTuple<DesignElement, Type>> GetElementTypes()
        {
            return Assembly.GetAssembly(typeof(PObject))
                .GetTypes()
                .Where(t => t.HasAttribute<DesignElement>())
                .Select(t => new AttributeTuple<DesignElement, Type>(t.GetAttribute<DesignElement>(), t));
        }
    }
}
