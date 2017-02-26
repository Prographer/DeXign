using DeXign.Extension;
using DeXign.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DeXign.Editor.Controls
{
    internal class IntellisenseInfoItemView : ListViewItem
    {
        public IntellisenseInfoItemModel Model => (IntellisenseInfoItemModel)DataContext;

        public string Category => Model.Category;
        
        public IntellisenseInfoItemView(IntellisenseInfoItemModel model)
        {
            this.DataContext = model;
        }
    }
}
