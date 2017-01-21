using System.Collections.Generic;

namespace DeXign.Core
{
    public interface IGenerator<TAttribute>
    {
        IEnumerable<string> Generate();
    }
}