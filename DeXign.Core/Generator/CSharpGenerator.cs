using DeXign.Core.Compiler;
using DeXign.Core.Logic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Reflection;
using DeXign.Extension;
using DeXign.Core.Text;

namespace DeXign.Core
{
    public class CSharpGenerator : Generator<CSharpCodeMapAttribute, PBinderHost>
    {
        public DXMapper<CSharpCodeMapAttribute> Mapper { get; private set; }

        public NameContainer NameContainer { get; private set; }
        public NameContainer CallbackContainer { get; private set; }

        public CSharpGenerator(
            CodeGeneratorUnit<PBinderHost> cgUnit,
            CodeGeneratorManifest cgManifest,
            CodeGeneratorAssemblyInfo cgAssmInfo) : base(cgUnit, cgManifest, cgAssmInfo)
        {
        }

        protected override IEnumerable<string> OnGenerate(IEnumerable<CodeComponent<CSharpCodeMapAttribute>> components)
        {
            var items = components.ToArray();
            CodeComponent<CSharpCodeMapAttribute> root = items.FirstOrDefault();

            // Trigger Hosts
            CodeComponent<CSharpCodeMapAttribute>[] triggers = items
                .Where(cc => cc.Depth == 0)
                .SelectMany(item => item.Children)
                .Distinct(new CodeComponentComparer<CSharpCodeMapAttribute>())
                .ToArray();

            // Event Handle
            var eventHandlerBuilder = new StringBuilder();

            foreach (var trigger in triggers)
            {
                var result = this.Mapper.Build(trigger.Element, trigger.Attribute.MappingCode);
                string source = result.Source;
                string callbackName = Mapper.MappingProvider.GetEventCallbackName(trigger.Element as PTrigger);
                
                source = source.Replace("{EventCallback}", callbackName);

                eventHandlerBuilder.AppendLine(source);
            }

            yield return eventHandlerBuilder.ToString();

            // Callback 
            foreach (var trigger in triggers)
            {
                string callbackSource = GenerateCallback(trigger.Element as PTrigger);
                
                // 지역 변수 컨테이너
                var localVariableContainer = new NameContainer();

                yield return callbackSource.Replace("{Code}", 
                    CreateScope(trigger.Children.ToArray(), localVariableContainer, true));
            }

            // CreateScope (Recursive)
            string CreateScope(CodeComponent<CSharpCodeMapAttribute>[] scopeComponents, NameContainer localVariableContainer, bool isCodeBlock = false)
            {
                var scopeBuilder = new IndentStringBuilder();
                var stack = new Stack<CodeComponent<CSharpCodeMapAttribute>>(scopeComponents);

                if (!isCodeBlock)
                {
                    scopeBuilder.AppendLine();
                    scopeBuilder.AppendLine("{");
                }

                while (stack.Count > 0)
                {
                    var cc = stack.Pop();

                    var mappingResult = this.Mapper.Build(cc.Element, cc.Attribute.MappingCode);
                    var sourceBuilder = new IndentStringBuilder(mappingResult.Source);

                    if (cc.Element is PBinderHost host)
                    {
                        foreach (DXToken token in mappingResult.Errors)
                        {
                            switch (token.Token)
                            {
                                case "Scope":
                                    {
                                        if (!token.IsIndexed)
                                            continue;

                                        int index = int.Parse(token.Parameter) - 1;

                                        var scopeBinder = host[BindOptions.Output].ElementAt(index) as PBinder;
                                        var childScopeComponents = scopeBinder.Items
                                            .Select(item => item.Host as PBinderHost)
                                            .Distinct()
                                            .Select(childHost => new CodeComponent<CSharpCodeMapAttribute>(
                                                childHost, childHost.GetAttribute<CSharpCodeMapAttribute>()))
                                            .ToArray();

                                        sourceBuilder.Replace(
                                            token.OriginalSource,
                                            CreateScope(childScopeComponents, localVariableContainer));

                                        break;
                                    }
                                    
                                case "Line":
                                    {
                                        if (!token.IsIndexed)
                                            continue;

                                        int index = int.Parse(token.Parameter) - 1;

                                        var paramBinder = host[BindOptions.Parameter].ElementAt(index) as PBinder;

                                        ProcessBinderValue(sourceBuilder, token, paramBinder);
                                        break;
                                    }

                                default:
                                    break;
                            }
                        }
                    }

                    scopeBuilder.AppendBlock(sourceBuilder.ToString(), 1);
                    scopeBuilder.AppendLine();
                }

                if (!isCodeBlock)
                    scopeBuilder.AppendLine("}");

                return scopeBuilder.ToString();
            }

            void ProcessBinderValue(IndentStringBuilder sourceBuilder, DXToken token, PBinder binder)
            {
                string valueLine = null;

                // 직접 입력
                if (binder.IsDirectValue)
                {
                    object value = binder.DirectValue;

                    valueLine = this.Mapper.MappingProvider.GetValueLine(value, true);
                }
                // 연결됨
                else if (binder.Items.Count == 1)
                {
                    var paramOutputHost = binder.Items[0].Host;

                    var attr = paramOutputHost.GetAttribute<CSharpCodeMapAttribute>();

                    if (paramOutputHost is PTrigger)
                        valueLine = "sender";
                    else
                        valueLine = this.Mapper.Build(paramOutputHost, attr.MappingCode).Source;
                }
                else
                {
                    // Error
                }

                if (valueLine != null)
                {
                    int idx = sourceBuilder.IndexOf(token.OriginalSource);

                    // 토큰 부분 제거
                    sourceBuilder.Replace(token.OriginalSource, "");

                    sourceBuilder.Insert(idx, valueLine);
                }
            }
        }

        private string GenerateCallback(PTrigger trigger)
        {
            var callbackBuilder = new IndentStringBuilder();

            string declareName = trigger.EventInfo.DeclaringType.GetAttribute<WPFAttribute>().Name;
            ParameterInfo[] infos = trigger.EventInfo.GetParameters();

            callbackBuilder.Append($"private void {CallbackContainer[trigger]}(");
            
            for (int i = 0; i < infos.Length; i++)
            {
                string paramName = infos[i].Name;

                if (paramName == "sender")
                    paramName = "oSender";

                callbackBuilder.Append(infos[i].ParameterType.Name);
                callbackBuilder.Append(" ");
                callbackBuilder.Append(paramName);

                if (i < infos.Length - 1)
                    callbackBuilder.Append(", ");
            }

            callbackBuilder.AppendLine(")");

            // Ex: private void Button_Click(object oSender, EventArgs)
            
            callbackBuilder.AppendLine("{");
            callbackBuilder.AppendLine($"var sender = oSender as {declareName};", 1);
            callbackBuilder.AppendLine();
            callbackBuilder.AppendLine("{Code}");
            callbackBuilder.Append("}");

            /*
            Ex: private void Button_Click(object oSender, EventArgs)
                {
                    var sender = oSender as {SenderType};
                {Code:1}
                }
            */

            return callbackBuilder.ToString();
        }

        public void SetNameContainer(NameContainer nameContainer)
        {
            this.NameContainer = nameContainer;
        }

        public void SetCallbackContainer(NameContainer callbackContainer)
        {
            this.CallbackContainer = callbackContainer;
        }

        public void SetMapper(DXMapper<CSharpCodeMapAttribute> mapper)
        {
            this.Mapper = mapper;
        }
    }
}
