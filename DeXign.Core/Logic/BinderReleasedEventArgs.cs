using System;

namespace DeXign.Core.Logic
{
    public class BinderReleasedEventArgs : EventArgs
    {
        public BinderExpression Expression { get; }

        public BinderReleasedEventArgs(BinderExpression expression)
        {
            this.Expression = expression;
        }
    }
}
