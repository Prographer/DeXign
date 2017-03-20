﻿using DeXign.Core.Designer;
using DeXign.Core.Logic;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        protected PParameterBinder TargetBinder { get; private set; }

        private ComboBox propertyBox;

        public BaseProperty()
        {
            SetValue(PropertyListPropertyKey, new ObservableCollection<PropertyItem>());

            TargetTypeProperty.AddValueChanged(this, TargetType_Changed);
            SelectedPropertyProperty.AddValueChanged(this, SelectedProperty_Changed);
        }
        
        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            this.TargetBinder = this.GetBinderModel<PParameterBinder>(BindOptions.Parameter, 0);

            this.Loaded += BaseProperty_Loaded;
        }

        private void BaseProperty_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= BaseProperty_Loaded;

            var popup = propertyBox.FindVisualChildrens<Popup>(false).FirstOrDefault(); ;
            var layer = this.Model.GetRenderer() as StoryboardLayer;

            layer?.Storyboard.SetUnscaledControl(popup);
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
            if (TargetType != null)
            {
                PropertyList.Clear();

                foreach (var prop in DesignerManager.GetProperties(TargetType))
                {
                    PropertyList.Add(new PropertyItem(prop));
                }

                propertyBox.SelectedIndex = 0;
            }

            OnTargetTypeChanged();
        }

        protected virtual void OnTargetTypeChanged()
        {
        }
    }
}
