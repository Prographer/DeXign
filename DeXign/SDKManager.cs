using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DeXign.SDK;
using DeXign.Extension;
using DeXign.Core.Designer;
using DeXign.Core;
using DeXign.Module.System;

namespace DeXign
{
    internal static class SDKManager
    {
        static AttributeTuple<DesignElementAttribute, Type>[] types;
        static Dictionary<Type, AttributeTuple<DesignElementAttribute, MethodInfo>[]> functions;

        static SDKManager()
        {
            // Reference Init
            DXSystem.Evaluate("0");

            types = GetModuleTypesCore().ToArray();

            functions = types
                .Select(at => at.Element)
                .ToDictionary(
                    t => t,
                    t => GetFunctionsCore(t).ToArray());
        }

        public static IEnumerable<AttributeTuple<DesignElementAttribute, Type>> GetModuleTypes()
        {
            return types;
        }

        public static IEnumerable<AttributeTuple<DesignElementAttribute, MethodInfo>> GetFunctions()
        {
            return functions.Values.SelectMany(v => v);
        }

        public static IEnumerable<AttributeTuple<DesignElementAttribute, MethodInfo>> GetFunctionsFromModule(Type moduleType)
        {
            if (functions.ContainsKey(moduleType))
                return functions[moduleType];

            return Enumerable.Empty<AttributeTuple<DesignElementAttribute, MethodInfo>>();
        }

        public static IEnumerable<AttributeTuple<DesignElementAttribute, Type>> GetModuleTypesCore()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assm => assm.GetTypes())
                .Where(t => t.HasAttribute<DXModuleAttribute>())
                .Select(t => new AttributeTuple<DesignElementAttribute, Type>(
                    Migration(t.GetAttribute<DXModuleAttribute>()), t));
        }

        private static IEnumerable<AttributeTuple<DesignElementAttribute, MethodInfo>> GetFunctionsCore(Type declareType)
        {
            return declareType.GetMethods()
                .Where(pi => pi.HasAttribute<DXFunctionAttribute>())
                .Select(mi =>
                {
                    var attr = Migration(mi.GetAttribute<DXFunctionAttribute>());
                    var tuple = new AttributeTuple<DesignElementAttribute, MethodInfo>(attr, mi);

                    attr.Category = mi.DeclaringType.GetAttribute<DXModuleAttribute>().DisplayName;

                    return tuple;
                });
        }

        public static DesignElementAttribute Migration(DXFunctionAttribute attr)
        {
            return new DesignElementAttribute()
            {
                DisplayName = attr.DisplayName
            };
        }

        public static DesignElementAttribute Migration(DXModuleAttribute attr)
        {
            return new DesignElementAttribute()
            {
                DisplayName = attr.DisplayName
            };
        }
    }
}