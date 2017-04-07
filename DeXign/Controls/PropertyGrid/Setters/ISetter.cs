using System;

namespace DeXign.Controls
{
    public interface ISetter : IDisposable
    {
        object Value { get; set; }

        bool IsStable { get; }
    }
}
