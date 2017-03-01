using System;

namespace DeXign.Core
{
    public class DesignElementIgnoreAttribute : Attribute
    {
        public string[] PropertyNames { get; set; }

        public DesignElementIgnoreAttribute(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }
    }
}
