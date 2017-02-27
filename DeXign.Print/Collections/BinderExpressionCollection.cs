using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DeXign.Core.Logic;

namespace DeXign.Core.Collections
{
    public class BinderExpressionCollection : Collection<BinderExpression>
    {
        public BinderExpressionCollection()
        {
        }

        public BinderExpressionCollection(IList<BinderExpression> list) : base(list)
        {
        }

        public IEnumerable<BinderExpression> GetExpressionsFromOutput(IBinder outputBinder)
        {
            return this.Where(be => be.Output.Equals(outputBinder));
        }

        public IEnumerable<BinderExpression> GetExpressionsFromInput(IBinder inputBinder)
        {
            return this.Where(be => be.Input.Equals(inputBinder));
        }

        public BinderExpression GetExpression(IBinder outputBinder, IBinder inputBinder)
        {
            return this.FirstOrDefault(be =>
                be.Output.Equals(outputBinder) && be.Input.Equals(inputBinder));
        }
    }
}
