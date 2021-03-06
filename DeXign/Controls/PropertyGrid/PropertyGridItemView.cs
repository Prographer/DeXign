using System.Windows.Controls;

using DeXign.Models;

namespace DeXign.Controls
{
    internal class PropertyGridItemView : ListViewItem
    {
        public PropertyGridItemModel Model => (PropertyGridItemModel)DataContext;

        public string Category => Model.Category;
        
        public PropertyGridItemView(PropertyGridItemModel model)
        {
            this.DataContext = model;
        }
    }
}
