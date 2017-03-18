using System;

namespace DeXign.Core
{
    public class ComponentParameterAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public Type AssignableType { get; set; }

        public int DisplayIndex { get; set; }

        public bool IsSingle { get; set; }

        public ComponentParameterAttribute(string displayName, Type assignableType)
        {
            this.DisplayName = displayName;
            this.AssignableType = assignableType;
        }
    }
}
