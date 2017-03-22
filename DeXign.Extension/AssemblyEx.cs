using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DeXign.Extension
{
    public static class AssemblyEx
    {
        public static string GetAssemblyLocation(string assemblyName)
        {
            assemblyName = Regex.Replace(assemblyName, @"\.dll$", "");

            Assembly assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .SingleOrDefault(ass => ass.GetName().Name.AnyEquals(assemblyName));

            return assembly?.Location;
        }
    }
}
