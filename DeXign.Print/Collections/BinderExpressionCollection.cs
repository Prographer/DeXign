using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DeXign.Logic.Collections
{
    public class BinderExpressionCollection : ObservableCollection<BinderExpression>
    {
        public BinderExpressionCollection()
        {
        }

        public BinderExpressionCollection(IEnumerable<BinderExpression> collection) : base(collection)
        {
        }

        public BinderExpressionCollection(List<BinderExpression> list) : base(list)
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
