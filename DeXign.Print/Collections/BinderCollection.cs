using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DeXign.Logic.Collections
{
    public class BinderCollection : ObservableCollection<IBinder>
    {
        public BinderCollection()
        {
        }

        public BinderCollection(IEnumerable<IBinder> collection) : base(collection)
        {
        }

        public BinderCollection(List<IBinder> list) : base(list)
        {
        }
    }
}
