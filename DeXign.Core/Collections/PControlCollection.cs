using System.Collections.Generic;
using System.Collections.ObjectModel;

using DeXign.Core.Controls;

namespace DeXign.Core.Collections
{
    public class PControlCollection<T> : ObservableCollection<T> 
        where T : PVisual
    {
        public PControlCollection()
        {
        }

        public PControlCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        public PControlCollection(List<T> list) : base(list)
        {
        }
    }

    public class PControlCollection : ObservableCollection<PControl>
    {
        public PControlCollection()
        {
        }

        public PControlCollection(IEnumerable<PControl> collection) : base(collection)
        {
        }

        public PControlCollection(List<PControl> list) : base(list)
        {
        }
    }
}