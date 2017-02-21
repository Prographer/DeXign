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

namespace DeXign.Controls
{
    internal class PropertyGrid : FilterListView
    {
        public static readonly DependencyProperty SelectedObjectsProperty =
            DependencyHelper.Register();

        public object[] SelectedObjects
        {
            get { return (object[])GetValue(SelectedObjectsProperty); }
            set { SetValue(SelectedObjectsProperty, value); }
        }

        DependencyObject presentedObject = null;
        
        public PropertyGrid() : base()
        {
            AddGroupProperty("Category");
            
            SelectedObjectsProperty.AddValueChanged(this, SelectedObjects_Changed);
        }

        private void SelectedObjects_Changed(object sender, EventArgs e)
        {
            foreach (PropertyGridItemModel item in this)
                if (item.Setter is IDisposable)
                    (item.Setter as IDisposable).Dispose();

            this.Clear();

            if (SelectedObjects != null && SelectedObjects.Length > 0 &&
                presentedObject != SelectedObjects[0])
            {
                presentedObject = (DependencyObject)SelectedObjects[0];
                
                foreach (var prop in DesignerManager.GetProperties(presentedObject.GetType()))
                {
                    ISetter setter;

                    if (string.IsNullOrEmpty(prop.Attribute.Key))
                        setter = SetterManager.CreateSetter(presentedObject, prop.Element);
                    else
                        setter = SetterManager.CreateSetter(presentedObject, prop.Element, prop.Attribute.Key);
                    
                    if (setter == null)
                        continue;
                    
                    this.AddItem(
                        new PropertyGridItemModel(prop, setter));
                }
            }
            else
            {
                presentedObject = null;
            }
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