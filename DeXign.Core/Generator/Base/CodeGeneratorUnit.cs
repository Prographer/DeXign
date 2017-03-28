using System;
using System.Collections.Generic;

namespace DeXign.Core
{
    public abstract class CodeGeneratorUnit<TElement> 
        where TElement : class, new()
    {
        public bool NodeIterating { get; set; } = true;
        public List<TElement> Items { get; }

        public CodeGeneratorUnit()
        {
            this.Items = new List<TElement>();
        }

        public CodeGeneratorUnit(IEnumerable<TElement> items) : this()
        {
            this.Items.AddRange(items);
        }

        public abstract IEnumerable<CodeComponent<TAttribute>> GetComponents<TAttribute>()
            where TAttribute : Attribute;
    }
}