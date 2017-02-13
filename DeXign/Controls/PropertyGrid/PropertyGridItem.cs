using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeXign.Core;
using DeXign.Core.Designer;

namespace DeXign.Controls
{
    internal class PropertyGridItem
    {
        internal AttributeTuple<DesignElementAttribute, Type> Metadata { get; }
        internal DesignerResource Resource { get; }

        public string Title { get; }

        public object Content => Resource.Content;

        public object ToolTip => Resource.ToolTip;

        public string Category => Metadata.Attribute.Category;

        public PropertyGridItem(AttributeTuple<DesignElementAttribute, Type> data, DesignerResource resource)
        {
            this.Metadata = data;
            this.Resource = resource;

            string displayName = Metadata.Attribute.DisplayName;

            // Default Display Name
            if (string.IsNullOrEmpty(displayName))
                displayName = Metadata.Element.GetType().Name;

            this.Title = displayName;
        }
    }
}
