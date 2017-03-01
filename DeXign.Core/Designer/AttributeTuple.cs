using System;

namespace DeXign.Core.Designer
{
    public class AttributeTuple<TAttribute, TElement>
        where TAttribute : Attribute
    {
        public TAttribute Attribute { get; set; }
        public TElement Element { get; set; }

        public AttributeTuple(TAttribute attr, TElement element)
        {
            this.Attribute = attr;
            this.Element = element;
        }

        public override string ToString()
        {
            return $"[{Attribute.ToString()}, {Element.ToString()}]";
        }
    }
}
