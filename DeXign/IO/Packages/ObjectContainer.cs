using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;

namespace DeXign.IO
{
    [Context("Items")]
    public class ObjectContainer<T>
    {
        public List<T> Items { get; set; }

        public ObjectContainer()
        {
            this.Items = new List<T>();
        }
    }
}
