using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace DeXign.Core.Collections
{
    public class PDefinitionCollection<T> : ObservableCollection<T>
        where T : IDefinition
    {
        public PDefinitionCollection()
        {
        }

        public PDefinitionCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        public PDefinitionCollection(List<T> list) : base(list)
        {
        }
    }
}
