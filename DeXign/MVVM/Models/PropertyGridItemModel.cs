using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeXign.Core;
using DeXign.Core.Designer;

namespace DeXign.Models
{
    internal class PropertyGridItemModel
    {
        internal AttributeTuple<DesignElementAttribute, Type> Metadata { get; }

        public string Category => Metadata.Attribute.Category;

        public PropertyGridItemModel(AttributeTuple<DesignElementAttribute, Type> data)
        {
            this.Metadata = data;
        }
    }
}
