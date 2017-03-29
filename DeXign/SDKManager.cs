using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using DeXign.SDK;
using DeXign.Extension;
using DeXign.Core.Designer;
using DeXign.Core;

namespace DeXign
{
    internal static class SDKManager
    {
        static AttributeTuple<DesignElementAttribute, Type>[] types;
        static Dictionary<Type, AttributeTuple<DesignElementAttribute, MethodInfo>[]> functions;

        static List<Assembly> loadedModules;

        static SDKManager()
        {
            SDKManager.Init();
            
            var sw = new Stopwatch();
            sw.Start();

            types = GetModuleTypesCore().ToArray();
            functions = types
                .Select(at => at.Element)
                .ToDictionary(
                    t => t,
                    t => GetFunctionsCore(t).ToArray());

            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds.ToString());
        }

        public static void Init()
        {
            if (loadedModules != null)
                return;

            loadedModules = new List<Assembly>();

            // Safe
            DirectoryEx.Create("Plugins");

            foreach (string fileName in Directory.GetFiles("Plugins", "*.dll", SearchOption.TopDirectoryOnly))
            {
                var moduleAssembly = Assembly.LoadFile(Path.GetFullPath(fileName));
                
                loadedModules.Add(moduleAssembly);
            }
        }

        public static IEnumerable<string> GetReferencedModules()
        {
            return loadedModules.Select(assm => assm.FullName);
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
            return loadedModules
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