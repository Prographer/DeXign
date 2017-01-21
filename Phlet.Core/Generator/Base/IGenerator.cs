using System.Collections.Generic;

namespace Phlet.Core
{
    public interface IGenerator<TAttribute>
    {
        IEnumerable<string> Generate();
    }
}