using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using WPFExtension;

namespace DeXign.Controls
{
    class FilterListView : ListView, IEnumerable<object>
    {
        private ObservableCollection<object> items;
        private CollectionView collectionView;

        public static readonly DependencyProperty FilterKeywordProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata("", FilterKeywordChanged));

        private static void FilterKeywordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FilterListView).Filter();
        }

        public int ItemCount
        {
            get { return items.Count; }
        }

        public string FilterKeyword
        {
            get { return this.GetValue<string>(FilterKeywordProperty); }
            set { SetValue(FilterKeywordProperty, value); }
        }

        public FilterListView()
        {
            items = new ObservableCollection<object>();
            this.ItemsSource = items;

            InitializeGrouping();
        }

        public virtual void Clear()
        {
            items.Clear();
        }

        public virtual void AddItem(object item)
        {
            items.Add(item);
        }

        public virtual void AddItems(IEnumerable<object> items)
        {
            foreach (object item in items)
                this.items.Add(item);
        }

        public virtual void RemoveItem(object item)
        {
            items.Remove(item);
        }

        public void AddGroupProperty(string propertyName)
        {
            collectionView?.GroupDescriptions?.Add(
                new PropertyGroupDescription(propertyName));
        }

        public void RemoveGroupProperty(string propertyName)
        {
            collectionView?.GroupDescriptions?.Remove(
                new PropertyGroupDescription(propertyName));
        }

        public void Filter()
        {
            collectionView?.Refresh();
        }

        internal void InitializeGrouping()
        {
            collectionView = (CollectionView)CollectionViewSource.GetDefaultView(this.ItemsSource);
            collectionView.Filter = OnFilter;
        }

        protected virtual bool OnFilter(object item)
        {
            return true;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
