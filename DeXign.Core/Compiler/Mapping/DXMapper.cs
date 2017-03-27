using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using DeXign.Extension;
using DeXign.Core.Logic;
using DeXign.Core.Controls;
using System.Windows;

namespace DeXign.Core.Compiler
{
    public class DXMapper
    {
        private static string[] SupportTokenHeaders =
            new string[]
            {
                "Target",
                "Property",
                "Function",
                //"Value",
                //"Scope",
                //"Line"
            };

        public IMappingProvider MappingProvider { get; }

        public NameContainer NameContainer { get; }

        public DXMapper(IMappingProvider provider, NameContainer nameContainer)
        {
            this.NameContainer = nameContainer;
            this.MappingProvider = provider;
        }

        public DXMappingResult Build(object obj, string source)
        {
            var result = new DXMappingResult();
            var sourceBuilder = new StringBuilder(source);
            
            // 이벤트 핸들러 연결 코드 및 콜백 이름 생성
            if (obj is PTrigger trigger)
            {
                var eventBuilder = new StringBuilder();

                string eventName = this.MappingProvider.GetEventName(trigger);

                source = source.Replace("{EventName}", eventName);
                
                foreach (var item in trigger.Items
                    .GetExpressions()
                    .Where(e => e.Direction == BindDirection.Input && e.Input.BindOption == BindOptions.Input))
                {
                    var targetHost = item.Output.Host as PLayoutBinderHost;

                    var hostResult = this.Build(targetHost, source);

                    eventBuilder.AppendLine(hostResult.Source);

                    foreach (DXToken errorToken in hostResult.Errors)
                        result.AddErrorToken(errorToken);

                    foreach (DXToken resolvedToken in hostResult.Resolved)
                        result.AddErrorToken(resolvedToken);
                }

                sourceBuilder = new StringBuilder(eventBuilder.ToString());
            }

            // 토큰 처리
            foreach (DXToken token in DXToken.Parse(sourceBuilder.ToString()))
            {
                if (!IsSupportToken(token))
                {
                    result.Errors.Add(token);
                    continue;
                }

                bool tokenResult = false;

                switch (token.Token)
                {
                    case "Property":
                        tokenResult = this.BuildProperty(token, obj, sourceBuilder);
                        break;

                    case "Function":
                        tokenResult = this.BuildFunction(token, obj, sourceBuilder);
                        break;

                    case "Target":
                        tokenResult = this.BuildTarget(token, obj, sourceBuilder);
                        break;
                }

                if (tokenResult)
                {
                    result.AddResolvedToken(token);
                }
                else
                {
                    result.AddErrorToken(token);

                    if (token.IsSafe)
                    {
                        sourceBuilder.Replace(token.OriginalSource, "");
                    }
                }
            }

            result.Source = sourceBuilder.ToString();

            return result;
        }

        private bool BuildProperty(DXToken token, object obj, StringBuilder source)
        {
            Type objType = obj.GetType();

            if (!token.HasParameter)
                return false;

            // 클래스가 아닌경우
            if (!obj.GetType().IsClass)
                return false;
            
            PropertyInfo pi = objType.GetProperty(token.Parameter);

            if (pi == null)
                return false;

            object value = pi.GetValue(obj);
            string valueLine = null;

            if (value is PBinder binder)
            {
                if (binder.IsDirectValue)
                {
                    valueLine = this.MappingProvider.GetValueLine(binder.DirectValue, true);
                }
                else
                {
                    if (binder.Items.Count == 0)
                        return false;

                    var previousHost = binder.Items[0].Host as PBinderHost;
                    var r = this.Build(previousHost, previousHost.GetAttribute<CodeMapAttribute>().MappingCode);

                    valueLine = r.Source;

                    if ((binder as PParameterBinder).Host is PSetter setter)
                    {
                        Type valueType = previousHost.GetType();

                        if (previousHost is PFunction prevFunc)
                            valueType = prevFunc.FunctionInfo.ReturnType;

                        if (previousHost is PGetter prevGetter)
                            valueType = prevGetter.Property.PropertyType;

                        if (previousHost is PLayoutBinderHost prevLayout)
                            valueType = prevLayout.LogicalParent.GetType();

                        if (previousHost is PSelector prevSelector)
                            valueType = prevSelector.TargetVisual.GetType();

                        if (setter.Property.PropertyType != valueType)
                        {
                            // casting
                            valueLine = $"{valueLine}.Cast<{setter.Property.PropertyType.Name}>()";
                        }
                    }
                }
            }
            else if (value is DependencyProperty prop)
            {
                PropertyInfo targetPi = prop.OwnerType.GetProperty(prop.Name);

                valueLine = this.MappingProvider.GetValueLine(targetPi);
            }
            else
            {
                valueLine = this.MappingProvider.GetValueLine(value);
            }

            if (valueLine != null)
            {
                // source 토큰 치환
                foreach (DXToken innerToken in DXToken.Parse(valueLine))
                    source.Replace(innerToken.OriginalSource, "");
                
                source.Replace(
                    token.OriginalSource,
                    valueLine);
            }

            return true;
        }

        private bool BuildFunction(DXToken token, object obj, StringBuilder source)
        {
            Type objType = obj.GetType();

            if (!token.HasParameter)
                return false;

            // 클래스가 아닌경우
            if (!obj.GetType().IsClass)
                return false;

            if (token.HasReturn)
            {
                // * 함수 정보에서 가져옴

                if (obj is PFunction pFunc)
                {
                    if (pFunc.Items.Find(BindOptions.Parameter).Count() != pFunc.ParameterInfos.Length)
                        return false;

                    List<string> lines = this.BuildBinderHost(pFunc);
                    
                    source.Replace(
                        token.OriginalSource,
                        this.MappingProvider.GetFunctionLine(pFunc, lines.ToArray()));
                }
            }
            else
            {
                // * 함수 호출 및 반환값으로 치환

                // 토큰 파라미터로 메서드 가져옴
                MethodInfo mi = objType.GetMethod(token.Parameter);

                // 지원하지 않는 메서드
                if (!IsSupportMethod(mi))
                    return false;

                // 메서드 호출
                string code = (string)mi.Invoke(obj, null);

                source.Replace(
                    token.OriginalSource, 
                    code);

                return true;
            }

            return false;
        }

        private bool BuildTarget(DXToken token, object obj, StringBuilder source)
        {
            PVisual targetVisual = null;

            if (obj is PLayoutBinderHost host)
                targetVisual = host.LogicalParent;

            if (obj is PSelector selector)
                targetVisual = selector.TargetVisual;

            if (obj is PTargetable targetable)
            {
                // 연결됨
                if (targetable.TargetBinder.Items.Count > 0)
                {
                    IBinderHost targetHost = targetable.TargetBinder.Items[0].Host;

                    // 레이아웃에서 직접 연결
                    if (targetHost is PLayoutBinderHost targetLayoutHost)
                        targetVisual = targetLayoutHost.LogicalParent;

                    // 선택기로 가져옴
                    if (targetHost is PSelector targetSelector)
                        targetVisual = targetSelector.TargetVisual;

                    // Target을 해결하지 못한경우는 무조건 이벤트 sender
                    if (targetVisual == null)
                    {
                        source.Replace(token.OriginalSource, "sender");
                    }
                }
            }

            if (this.NameContainer.ContainsValue(targetVisual))
            {
                string name = this.NameContainer[targetVisual].Trim('_');

                source.Replace(
                    token.OriginalSource, 
                    name);

                return true;
            }
            
            return false;
        }

        private List<string> BuildBinderHost(PBinderHost rootHost)
        {
            var parameterLines = new List<string>();

            // 연결된 파라미터 바인더 가져옴
            foreach (var expression in rootHost.Items.GetExpressions()
                .Where(e => e.Direction == BindDirection.Input && e.Input.BindOption == BindOptions.Parameter))
            {
                var host = expression.Output.Host as PBinderHost;

                // 파라미터 역전파로 트리거에 도달하는 경우
                // 무조건 sender를 가져옴
                if (host is PTrigger)
                {
                    parameterLines.Add("sender");
                    continue;
                }

                if (host.HasAttribute<CodeMapAttribute>())
                {
                    var attr = host.GetAttribute<CodeMapAttribute>();

                    DXMappingResult result = this.Build(host, attr.MappingCode);

                    parameterLines.Add(result.Source);
                }
            }

            return parameterLines;
        }

        public static bool IsSupportToken(DXToken token)
        {
            return SupportTokenHeaders.Contains(token.Token);
        }

        public static bool IsSupportMethod(MethodInfo mi)
        {
            if (mi == null)
                return false;

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
