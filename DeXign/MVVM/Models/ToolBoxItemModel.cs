using System;

using DeXign.Core;
using DeXign.Core.Designer;

namespace DeXign.Models
{
    internal class ToolBoxItemModel
    {
        internal AttributeTuple<DesignElementAttribute, Type> Metadata { get; }
        internal DesignerResource Resource { get; }

        public string Title { get; }

        public object Content => Resource?.Content;
        
        public object ToolTip { get; }

        public string Category { get; }

        // 일반 레이아웃 및 컴포넌트
        public ToolBoxItemModel(AttributeTuple<DesignElementAttribute, Type> data, DesignerResource resource)
        {
            this.Metadata = data;
            this.Resource = resource;
            this.ToolTip = this.Resource.ToolTip;
            this.Category = Metadata.Attribute.Category;

            string displayName = Metadata.Attribute.DisplayName;

            // Default Display Name
            if (string.IsNullOrEmpty(displayName))
                displayName = Metadata.Element.GetType().Name;
            
            this.Title = displayName;
        }
    }
}
