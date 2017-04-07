using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

using DeXign.Models;
using DeXign.Resources;
using DeXign.Core.Designer;

using WPFExtension;

using Moda.KString;
using DeXign.Extension;
using System.Windows.Data;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DeXign.Core;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_searchBox", Type = typeof(TextBox))]
    internal class PropertyGrid : FilterListView
    {
        public static readonly DependencyProperty SelectedObjectsProperty =
            DependencyHelper.Register();

        public DependencyObject[] SelectedObjects
        {
            get { return this.GetValue<DependencyObject[]>(SelectedObjectsProperty); }
            set { SetValue(SelectedObjectsProperty, value); }
        }

        public PropertyGrid() : base()
        {
            AddGroupProperty("Category");
            
            SelectedObjectsProperty.AddValueChanged(this, SelectedObjects_Changed);
        }
        
        private void SelectedObjects_Changed(object sender, EventArgs e)
        {
            foreach (PropertyGridItemModel item in this)
                if (item.Setter is IDisposable disposable)
                    disposable.Dispose();

            this.Clear();

            if (SelectedObjects?.Length > 0)
            {
                foreach (var group in SelectedObjects
                    .SelectMany(obj => GetHashedProperties(obj as DependencyObject))
                    .GroupBy(
                        p => p.HashCode,
                        p => p))
                {
                    if (group.Count() == SelectedObjects.Length)
                    {
                        var prop = group.ElementAt(0).Value;

                        PropertyInfo[] props = group
                            .Select(item => item.Value.Element)
                            .ToArray();

                        // »ý¼º
                        ISetter setter;

                        if (string.IsNullOrEmpty(prop.Attribute.Key))
                            setter = SetterManager.CreateSetter(SelectedObjects, props);
                        else
                            setter = SetterManager.CreateSetter(SelectedObjects, props, prop.Attribute.Key);

                        if (setter == null)
                            continue;

                        this.AddItem(
                            new PropertyGridItemModel(prop, setter));
                    }
                }
            }
        }

        private IEnumerable<(int HashCode, AttributeTuple<DesignElementAttribute, PropertyInfo> Value)> GetHashedProperties(DependencyObject obj)
        {
            return DesignerManager.GetProperties(obj.GetType())
                .Select(p => (CreatePropertyHash(p), p));
        }

        private int CreatePropertyHash(AttributeTuple<DesignElementAttribute, PropertyInfo> attr)
        {
            return $"{attr.Attribute.Key}{attr.Attribute.DisplayName}{attr.Element.PropertyType.MetadataToken}".GetHashCode();
        }

        private void SetPresentedObject(DependencyObject obj)
        {
            this.DataContext = obj;
            //this.presentedObject = obj;
        }

        protected override bool OnFilter(object item)
        {
            return (item as PropertyGridItemModel).Title.KContains(FilterKeyword);
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

            // Search bar Binding
            Binding b = BindingHelper.SetBinding(
                GetTemplateChild("PART_searchBox"), TextBox.TextProperty,
                this, FilterKeywordProperty,
                sourceTrigger: UpdateSourceTrigger.PropertyChanged);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (this.View == null)
                return;

            var gridView = (GridView)this.View;

            double totColumnWidth = gridView
                .Columns
                .Cast<StarGridViewColumn>()
                .Sum(c => c.StarWidth);

            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                var column = (StarGridViewColumn)gridView.Columns[i];

                column.Width = column.StarWidth / totColumnWidth * sizeInfo.NewSize.Width;
            }
        }
    }
}