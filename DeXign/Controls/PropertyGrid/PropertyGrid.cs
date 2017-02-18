using System.ComponentModel;
using System.Windows.Controls;

using DeXign.Resources;

namespace DeXign.Controls
{
    internal class PropertyGrid : FilterListView
    {
        public PropertyGrid() : base()
        {
            AddGroupProperty("Category");
        }

        protected override bool OnFilter(object item)
        {
            //return (item as ToolBoxItem).Title.KContains(FilterKeyword);
            return true;
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
