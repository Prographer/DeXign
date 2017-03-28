using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using DeXign.Core.Designer;
using DeXign.Core.Logic;
using DeXign.Extension;

using WPFExtension;

namespace DeXign.Editor.Logic
{
    [TemplatePart(Name = "PART_propertyBox", Type = typeof(ComboBox))]
    public abstract class BaseProperty : ComponentElement
    {
        public static readonly DependencyProperty SelectedPropertyProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty TargetTypeProperty =
            DependencyHelper.Register();

        private static DependencyPropertyKey PropertyListPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static DependencyProperty PropertyListProperty =
            PropertyListPropertyKey.DependencyProperty;

        public DependencyProperty SelectedProperty
        {
            get { return (DependencyProperty)GetValue(SelectedPropertyProperty); }
            set { SetValue(SelectedPropertyProperty, value); }
        }

        public Type TargetType
        {
            get { return (Type)GetValue(TargetTypeProperty); }
            set { SetValue(TargetTypeProperty, value); }
        }

        public ObservableCollection<PropertyItem> PropertyList
        {
            get { return (ObservableCollection<PropertyItem>)GetValue(PropertyListProperty); }
        }

        public new PTargetable Model => (PTargetable)base.Model;

        protected Dictionary<DependencyProperty, PropertyItem> dictItems;
        private ComboBox propertyBox;

        public BaseProperty()
        {
            dictItems = new Dictionary<DependencyProperty, PropertyItem>();

            SetValue(PropertyListPropertyKey, new ObservableCollection<PropertyItem>());

            TargetTypeProperty.AddValueChanged(this, TargetType_Changed);
            SelectedPropertyProperty.AddValueChanged(this, SelectedProperty_Changed);
        }
        
        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            BindingEx.SetBinding(
                this.Model, PTargetable.PropertyProperty,
                this, BaseProperty.SelectedPropertyProperty);
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            var popup = propertyBox.FindVisualChildrens<Popup>(false).FirstOrDefault(); ;

            this.ParentStoryboard.SetUnscaledControl(popup);
        }

        public override void OnApplyContentTemplate()
        {
            base.OnApplyContentTemplate();

            propertyBox = GetContentTemplateChild<ComboBox>("PART_propertyBox");
            
            propertyBox.SelectionChanged += PropertyBox_SelectionChanged;
        }

        private void PropertyBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (propertyBox.SelectedItem is PropertyItem item)
            {
                this.SelectedProperty = item.Property;
            }
        }

        private void SelectedProperty_Changed(object sender, EventArgs e)
        {
            OnSelectedPropertyChanged();
        }

        protected virtual void OnSelectedPropertyChanged()
        {
        }

        private void TargetType_Changed(object sender, EventArgs e)
        {
            UpdateTargetType();
        }

        private void UpdateTargetType()
        {
            if (TargetType == null)
            {
                this.Clear();
            }
            else
            {
                PropertyList.Clear();
                
                foreach (var prop in DesignerManager.GetProperties(TargetType))
                {
                    var item = new PropertyItem(prop);

                    dictItems[item.Property] = item;

                    PropertyList.Add(item);
                }

                if (this.Model.Property != null && dictItems.ContainsKey(this.Model.Property))
                {
                    propertyBox.SelectedItem = dictItems[this.Model.Property];
                }
                else
                {
                    propertyBox.SelectedIndex = 0;
                }
            }

            OnTargetTypeChanged();
        }

        protected virtual void OnTargetTypeChanged()
        {
        }
        
        protected void Clear()
        {
            this.TargetType = null;
            this.PropertyList.Clear();
            dictItems.Clear();
        }
    }
}
