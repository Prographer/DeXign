using System.ComponentModel;
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

        private void OnDragEnd(object sender, DragHelper.DragEventArgs e)
        {
            // TODO: End Drag
        }

        private void OnDrag(object sender, DragHelper.DragEventArgs e)
        {
            // TODO: Drag
        }

        protected override bool OnFilter(object item)
        {
            return (item as ToolBoxItemView).Model.Title.KContains(FilterKeyword);
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
