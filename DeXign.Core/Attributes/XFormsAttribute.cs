using System;

namespace DeXign.Core
{
    public class XFormsAttribute : Attribute
    {
        public string Namespace { get; set; }
        public string Name { get; set; }

        public XFormsAttribute(string @namespace, string name)
        {
            this.Namespace = @namespace;
            this.Name = name;
        }

        public XFormsAttribute(string name)
        {
            this.Name = name;
        }
    }
}