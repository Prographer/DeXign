using DeXign.Core.Logic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DeXign.Core.Collections
{
    public class BinderCollection : ObservableCollection<IBinder>
    {
        public bool IsHost { get; }

        public IBinder Binder { get; }

        public BinderCollection(IBinder binder)
        {
            this.Binder = binder;
            this.IsHost = (binder == null);
        }

        public IEnumerable<IBinder> Find(BindOptions option)
        {
            return this.Where(b => option.HasFlag(b.BindOption));
        }

        public IEnumerable<IBinder> Find(BindDirection direction)
        {
            if (!this.IsHost)
                throw new System.Exception();

            return Find((BindOptions)direction);
        }

        public IEnumerable<(IBinder Output, IBinder Input, BindDirection Direction)> GetExpressions()
        {
            if (this.IsHost)
            {
                return this.SelectMany(binder => binder.Items.GetExpressions());
            }
            else
            {
                return Items.Select(target =>
                {
                    if (BindDirection.Output.HasFlag((BindDirection)Binder.BindOption))
                    {
                        return (Binder, target, BindDirection.Output);
                    }

                    return (target, Binder, BindDirection.Input);
                });
            }
        }

        public bool HasConnection(IBinder output, IBinder input)
        {
            return GetExpressions()
                .Count(expression =>
                {
                    return expression.Output.Equals(output) &&
                           expression.Input.Equals(input);
                }) > 0;
        }
    }
}