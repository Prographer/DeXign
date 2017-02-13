using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Moda.KString;
using DeXign.Resources;

namespace DeXign.Controls
{
    internal class ToolBox : FilterListView
    {
        public ToolBox() : base()
        {
            AddGroupProperty("Category");
        }

        protected override bool OnFilter(object item)
        {
            return (item as ToolBoxItem).Title.KContains(FilterKeyword);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            this.GroupStyle.Add(new GroupStyle()
            {
                ContainerStyle = ResourceManager.GetStyle("DeXignGroupItemStyle")
            });
        }
    }
}
