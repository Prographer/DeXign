using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFExtension;

namespace DeXign.Controls
{
    internal class ToolBox : ListView
    {
        private ObservableCollection<ToolBoxItem> items;
        private CollectionView collectionView;

        public static readonly DependencyProperty FilterKeywordProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(FilterKeywordChanged));

        private static void FilterKeywordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ToolBox).Filter();
        }

        public string FilterKeyword
        {
            get { return (string)GetValue(FilterKeywordProperty); }
            set { SetValue(FilterKeywordProperty, value); }
        }

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

        public void Filter()
        {
            collectionView.Refresh();
        }

        internal void InitializeGrouping()
        {
            collectionView = (CollectionView)CollectionViewSource.GetDefaultView(this.ItemsSource);

            if (collectionView.GroupDescriptions?.Count == 0)
                collectionView.GroupDescriptions.Add(
                    new PropertyGroupDescription("Category"));

            collectionView.Filter = Filter;
        }

        private bool Filter(object obj)
        {
            if (obj is ToolBoxItem)
            {
                var item = obj as ToolBoxItem;

                return HangulLib.Hangul.Contains(item.Title, FilterKeyword);
            }

            return false;
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
