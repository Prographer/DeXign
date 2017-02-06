using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DeXign.Controls
{
    internal class ToolBox : ListView
    {
        private ObservableCollection<ToolBoxItem> items;

        public ToolBox()
        {
            items = new ObservableCollection<ToolBoxItem>();
            this.ItemsSource = items;

            InitializeGrouping();
        }

        public void AddItem(ToolBoxItem item)
        {
            items.Add(item);
        }

        public void RemoveItem(ToolBoxItem item)
        {
            items.Remove(item);
        }

        internal void InitializeGrouping()
        {
            var view = (CollectionView)CollectionViewSource.GetDefaultView(this.ItemsSource);

            if (view.GroupDescriptions?.Count == 0)
                view.GroupDescriptions.Add(
                    new PropertyGroupDescription("Category"));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            this.GroupStyle.Add(new GroupStyle()
            {
                ContainerStyle = FindResource("DeXignGroupItemStyle") as Style
            });
        }
    }
}
