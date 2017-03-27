using System;
using System.Text;

using DeXign.Core.Logic;
using DeXign.Extension;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Core.Compiler
{
    public class WPFMappingProvider : IMappingProvider
    {
        public NameContainer CallbackContainer { get; }

        public WPFMappingProvider(NameContainer callbackContainer)
        {
            this.CallbackContainer = callbackContainer;
        }

        public string GetEventName(PTrigger trigger)
        {
            return trigger.EventInfo.RuntimeEventInfo.GetAttribute<WPFAttribute>().Name;
        }

        public string GetEventCallbackName(PTrigger trigger)
        {
            string eventName = this.GetEventName(trigger);
            string declareName = trigger.EventInfo.DeclaringType.GetAttribute<WPFAttribute>().Name;

            var paramterBuilder = new StringBuilder();
            string callbackName = CallbackContainer.GetName(trigger, $"{declareName}_{eventName}");
            
            // 콜백 이름 등록
            CallbackContainer[callbackName] = trigger;
            
            return callbackName;
        }

        public string GetFunctionLine(PFunction pFunc, params string[] parameters)
        {
            ParameterInfo[] infos = pFunc.FunctionInfo.GetParameters();
            var sb = new StringBuilder();

            sb.Append(pFunc.FunctionInfo.DeclaringType.Name);
            sb.Append(".");
            sb.Append(pFunc.FunctionInfo.Name);
            sb.Append("(");

            for (int i = 0; i < parameters.Length; i++)
            {
                sb.Append($"({infos[i].ParameterType.Name}){parameters[i]}");

                if (i < parameters.Length - 1)
                    sb.Append(", ");
            }

            sb.Append(")");

            return sb.ToString();
        }

        public string GetMappingCode(object obj)
        {
            if (obj.HasAttribute<CodeMapAttribute>())
                return obj.GetAttribute<CodeMapAttribute>().MappingCode;

            return null;
        }

        public string GetNullString()
        {
            return "null";
        }

        public string GetValueLine(object obj, bool isInline = false)
        {
            Type objType = obj.GetType();

            if (obj is string && isInline)
                return $"\"{obj}\"";
            
            if (obj.HasAttribute<WPFAttribute>())
            {
                var sb = new StringBuilder();
                var attr = obj.GetAttribute<WPFAttribute>();
                
                // 특성이 정의된 Enum
                if (objType.IsEnum && objType.HasAttribute<WPFAttribute>())
                {
                    var enumAttr = objType.GetAttribute<WPFAttribute>();

                    return $"{enumAttr.Name}.{attr.Name}";
                }

                return attr.Name;
            }

            if (obj is SolidColorBrush)
            {
                return $"\"{obj.ToString()}\".ToBrush()";
            }

            return obj?.ToString();
        }
    }
}