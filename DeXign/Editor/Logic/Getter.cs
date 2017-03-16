using System;
using System.Windows;
using System.Reflection;
using System.Collections.ObjectModel;

using DeXign.Core;
using DeXign.Extension;
using DeXign.Core.Designer;

using WPFExtension;
using DeXign.Core.Logic;
using System.Linq;
using System.Windows.Controls;

namespace DeXign.Editor.Logic
{
    public class PropertyItem
    {
        public DesignElementAttribute Metadata { get; }
        public DependencyProperty Property { get; }

        public string Title => Metadata.DisplayName;

        public PropertyItem(AttributeTuple<DesignElementAttribute, PropertyInfo> data)
        {
            this.Metadata = data.Attribute;
            this.Property = data.Element.GetDependencyProperty();
        }
    }
    
    public class Getter : ComponentElement
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

        protected PParameterBinder TargetBinder { get; private set; }

        private ComboBox propertyBox;

        public Getter()
        {
            SetValue(PropertyListPropertyKey, new ObservableCollection<PropertyItem>());

            TargetTypeProperty.AddValueChanged(this, Target_Changed);
        }

        public override void OnApplyContentTemplate()
        {
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

        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            if (this.TargetBinder != null)
                return;

            IBinder binder = base.Model[BindOptions.Parameter].First();

            if (binder is PParameterBinder paramBinder)
            {
                this.TargetBinder = paramBinder;

                this.TargetBinder.Binded += TargetBinder_Binded;
            }
        }

        private void TargetBinder_Binded(object sender, IBinder e)
        {
            // * PTrigger (Event)
            

            // * PVisual (View Model)

        }

        private void Target_Changed(object sender, EventArgs e)
        {
            if (TargetType != null)
            {
                PropertyList.Clear();

                foreach (var prop in DesignerManager.GetProperties(TargetType))
                {
                    PropertyList.Add(new PropertyItem(prop));
                }

                propertyBox.SelectedIndex = 0;
            }
        }
    }
}