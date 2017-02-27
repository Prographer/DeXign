using DeXign.Core.Logic;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DeXign.Core.Collections
{
    public class BinderCollection : Collection<IBinder>
    {
        public BinderCollection()
        {
        }

        public BinderCollection(IList<IBinder> list) : base(list)
        {
        }
    }
}
