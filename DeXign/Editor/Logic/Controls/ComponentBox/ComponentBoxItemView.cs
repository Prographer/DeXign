using DeXign.Models;
using System.Windows.Controls;

namespace DeXign.Editor.Logic
{
    internal class ComponentBoxItemView : ListViewItem
    {
        public ComponentBoxItemModel Model => (ComponentBoxItemModel)DataContext;

        public string Category => Model.Category;
        
        public ComponentBoxItemView(ComponentBoxItemModel model)
        {
            this.DataContext = model;
        }
    }
}
