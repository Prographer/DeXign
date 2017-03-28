using System;
using System.Collections.Generic;

namespace DeXign.Core
{
    public class CodeComponentComparer<TAttribute> : IEqualityComparer<CodeComponent<TAttribute>>
        where TAttribute : Attribute
    {
        public bool Equals(CodeComponent<TAttribute> x, CodeComponent<TAttribute> y)
        {
            return x.Element.Equals(y.Element);
        }

        public int GetHashCode(CodeComponent<TAttribute> obj)
        {
            return obj.Element.GetHashCode();
        }
    }
}
