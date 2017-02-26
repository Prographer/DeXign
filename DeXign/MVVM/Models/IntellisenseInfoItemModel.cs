using System;
using System.Reflection;

using DeXign.Core;
using DeXign.Core.Designer;

namespace DeXign.Models
{
    class IntellisenseInfoItemModel
    {
        public string Title { get; }

        public string Category { get; }

        internal IntellisenseInfoItemModel(DesignElementAttribute attr)
        {
            this.Title = attr.DisplayName;
            this.Category = attr.Category;
        }

        public IntellisenseInfoItemModel(AttributeTuple<DesignElementAttribute, EventInfo> data) 
            : this(data.Attribute)
        {   
        }

        public IntellisenseInfoItemModel(AttributeTuple<DesignElementAttribute, Type> data)
            : this(data.Attribute)
        {
        }
    }
}
