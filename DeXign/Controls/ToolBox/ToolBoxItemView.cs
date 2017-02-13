using System.Windows.Controls;

namespace DeXign.Controls
{
    class ToolBoxItemView : ListViewItem
    {
        public ToolBoxItemModel Model { get { return (ToolBoxItemModel)DataContext; } }

        public string Category => Model.Category;

        public ToolBoxItemView(ToolBoxItemModel item)
        {
            this.DataContext = item;
        }
    }
}
