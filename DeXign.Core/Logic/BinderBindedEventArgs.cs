using System;

namespace DeXign.Core.Logic
{
    public class BinderBindedEventArgs : EventArgs
    {
        public BinderExpression Expression { get; }

        public BinderBindedEventArgs(BinderExpression expression)
        {
            this.Expression = expression;
        }
    }
}
