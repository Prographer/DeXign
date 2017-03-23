using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using DeXign.Core.Logic;
using DeXign.Core.Controls;

namespace DeXign.Core.Compiler
{
    public class CSBuilder
    {
        public DXCompileOption Option { get; }
        public PContentPage[] Screens { get; }
        public PBinderHost[] Components { get; }

        private Dictionary<string, MethodInfo> methodsCache;

        public CSBuilder(DXCompileOption option, PContentPage[] screens, PBinderHost[] components)
        {
            this.Option = option;
            this.Screens = screens;
            this.Components = components;

            methodsCache = this.GetType().GetMethods()
                .Where(mi => IsSupportMethod(mi))
                .ToDictionary(
                    mi => mi.Name,
                    mi => mi);
        }

        // {RootNamespace}    // Property Matching
        // {ApplicationName}  // Property Matching
        // {Function Name}    // Function (only)
        public virtual string Build(string source)
        {
            (string Key, string Token)[] tokens = Regex.Matches(source, @"{(?<Key>.*?)}")
                .OfType<Match>()
                .Select(m => (m.Groups[1].Value.Trim(), m.Groups[0].Value))
                .ToArray();
            
            foreach (var token in tokens)
            {
                if (methodsCache.ContainsKey(token.Key))
                {
                    string code = (string)methodsCache[token.Key].Invoke(this, null);

                    source = source.Replace(token.Token, code);
                }
            }

            source = source.Replace("{RootNamespace}", this.Option.RootNamespace);
            source = source.Replace("{ApplicationName}", this.Option.ApplicationName);

            return source;
        }

        private bool IsSupportMethod(MethodInfo mi)
        {
            ParameterInfo[] infos = mi.GetParameters();

            if (mi.IsStatic)
                return false;

            if (mi.ReturnType != typeof(string))
                return false;

            if (infos.Length > 0)
                return false;
            
            return true;
        }
    }
}
