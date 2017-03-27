using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DeXign.Extension;
using DeXign.Core.Logic;
using DeXign.Core.Text;

namespace DeXign.Core.Compiler
{
    internal class WPFCodeBuilder
    {
        public DXCompileParameter Parameter { get; }
        public CSharpGenerator NativeGenerator { get; }
        public DXMapper Mapper => this.NativeGenerator.Mapper;

        private List<string> definedNamespaces;
        private List<string> sdkNamespaces;

        private List<string> logicCodes;

        public WPFCodeBuilder(DXCompileParameter parameter, CSharpGenerator generator)
        {
            this.Parameter = parameter;
            this.NativeGenerator = generator;

            sdkNamespaces =
                parameter.Components
                    .SelectMany(c => BinderHelper.FindHostNodes(c))
                    .Where(host => host is PFunction)
                    .Select(host => (host as PFunction).FunctionInfo.DeclaringType.Namespace)
                    .Distinct()
                    .ToList();
            
            definedNamespaces = new List<string>();
            logicCodes = new List<string>();
        }

        public DXMappingResult Build(string cs)
        {
            var result = new DXMappingResult();

            var r1 = this.Mapper.Build(this, cs);
            var r2 = this.Mapper.Build(this.Parameter.Option, r1.Source);

            // 정의된 네임스페이스 초기화
            definedNamespaces.Clear();

            definedNamespaces.AddRange(
                ParseDefinedNamespaces(cs));

            // 매핑 결과 두개 결합
            result.Source = r2.Source;

            result.Errors.AddRange(
                r1.Errors.Except(r2.Resolved)
                .Concat(
                    r2.Errors.Except(r1.Resolved)));

            result.Resolved.AddRange(r1.Resolved.Concat(r2.Resolved));

            return result;
        }

        // CodeMapping Function
        public string ComponentInitialize()
        {
            var indentBuilder = new IndentStringBuilder(3);
            
            foreach (var kv in this.Mapper.NameContainer)
            {
                var attr = kv.Value.GetAttribute<WPFAttribute>();
                string name = kv.Key.Trim('_');

                indentBuilder.AppendLine($"{name} = this.FindElement<{attr.Name}>(\"{name}\");");
            }

            logicCodes.Clear();
            logicCodes.AddRange(this.NativeGenerator.Generate().ToArray());

            if (logicCodes.Count > 0)
            {
                indentBuilder.AppendLine();
                indentBuilder.AppendLine("// Event Handlers");

                indentBuilder.AppendBlock(logicCodes[0]);
            }

            return indentBuilder.ToString();
        }

        // CodeMapping Function
        public string CallbackHandlers()
        {
            if (logicCodes.Count > 1)
            {
                var indentBuilder = new IndentStringBuilder(2);
                
                for (int i = 1; i < logicCodes.Count; i++)
                {
                    indentBuilder.AppendBlock(logicCodes[i]);

                    if (i < logicCodes.Count - 1)
                    {
                        indentBuilder.AppendLine();
                        indentBuilder.AppendLine();
                    }
                }

                return indentBuilder.ToString();
            }

            return null;
        }

        // CodeMapping Function
        public string GlobalVariableDefine()
        {
            var indentBuilder = new IndentStringBuilder(2);
            
            for (int i = 0; i < this.Mapper.NameContainer.Count; i++)
            {
                var kv = this.Mapper.NameContainer.ElementAt(i);
                var attr = kv.Value.GetAttribute<WPFAttribute>();

                indentBuilder.Append($"private {attr.Name} {kv.Key.Trim('_')};");

                if (i < this.Mapper.NameContainer.Count - 1)
                    indentBuilder.AppendLine();
            }

            return indentBuilder.ToString();
        }

        // CodeMapping Function
        public string NamespaceDefine()
        {
            var sb = new StringBuilder();
            
            // namespace list
            foreach (string ns in this.Mapper.NameContainer.Values
                .Select(obj => obj.GetAttribute<WPFAttribute>().Namespace)
                .Select(ns => GetCleanNamespace(ns))
                .Concat(sdkNamespaces)
                .Distinct())
            {
                string nsLine = $"using {ns};";

                if (definedNamespaces.Contains(nsLine))
                    continue;

                sb.AppendLine(nsLine);
            }

            return sb.ToString();
        }

        // CodeMapping Function
        public string PageInitialize()
        {
            var indentBuilder = new IndentStringBuilder(3);

            for (int i = 0; i < this.Parameter.Screens.Length; i++)
            {
                /* Ex
                var p0 = GenResourceManager.LoadXaml("Screen1") as DeXignPage;
                dw.Add(p0);
                */

                indentBuilder.AppendLine($"var p{i} = GenResourceManager.LoadXaml(\"{this.Parameter.Screens[i].GetPageName()}.xaml\") as DeXignPage;");
                indentBuilder.AppendLine($"dw.Add(p{i});");
            }

            /* Ex
            dw.SetPage(p0);
            */
            indentBuilder.Append($"dw.SetPage(p0);");

            return indentBuilder.ToString();
        }

        private string[] ParseDefinedNamespaces(string cs)
        {
            return Regex.Matches(cs, "using .+;")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();
        }

        private string GetCleanNamespace(string namespaceLine)
        {
            string pattern = @"(?<=clr-namespace:).+?(?=;)";

            if (Regex.IsMatch(namespaceLine, pattern))
                return Regex.Match(namespaceLine, pattern).Value;

            return namespaceLine;
        }
    }
}
