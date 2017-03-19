using System;
using System.Reflection;

namespace DeXign.Core
{
    public class NamedParameterInfo
    {
        public string Name { get; }
        public Type ParameterType { get; }

        public NamedParameterInfo(string name, Type parameterType)
        {
            this.Name = name;
            this.ParameterType = parameterType;
        }

        public NamedParameterInfo(ParameterInfo info)
        {
            this.Name = info.Name;
            this.ParameterType = info.ParameterType;
        }
    }
}
