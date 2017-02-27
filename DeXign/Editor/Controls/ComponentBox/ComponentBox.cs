using System;
using System.Windows;

using DeXign.Controls;
using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Models;

using WPFExtension;

namespace DeXign.Editor.Controls
{
    class ComponentBox : FilterListView
    {
        public event EventHandler<ComponentBoxItemModel> ItemSelected;

        public static readonly DependencyProperty TargetObjectProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty PlaceDirectionProperty =
            DependencyHelper.Register(
                new PropertyMetadata(Direction.Left));

        public static readonly DependencyProperty IsEmptyProperty =
            DependencyHelper.Register();

        public PObject TargetObject
        {
            get { return (PObject)GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }

        public Direction PlaceDirection
        {
            get { return (Direction)GetValue(PlaceDirectionProperty); }
            set { SetValue(PlaceDirectionProperty, value); }
        }

        public bool IsEmpty
        {
            get { return (bool)GetValue(IsEmptyProperty); }
            set { SetValue(IsEmptyProperty, value); }
        }
        
        private PObject presentedObject = null;

        public ComponentBox()
        {
            TargetObjectProperty.AddValueChanged(this, TargetObject_Changed);
        }

        protected override void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = SelectedItem as ComponentBoxItemView;

            if (item == null)
                return;

            ItemSelected?.Invoke(this, item.Model);

            this.UnselectAll();
        }

        private void TargetObject_Changed(object sender, EventArgs e)
        {
            if (TargetObject != null && !TargetObject.Equals(presentedObject))
            {
                presentedObject = TargetObject;

                this.Clear();

                foreach (var ev in DesignerManager.GetEvents(TargetObject.GetType()))
                {
                    var item = new ComponentBoxItemView(
                        new ComponentBoxItemModel(ev));
                    
                    this.AddItem(item);
                }
            }

            IsEmpty = this.ItemCount == 0;
        }

        public override void Clear()
        {
            foreach (var item in this)
                DestroyItem(item as ComponentBoxItemView);

            base.Clear();
        }

        public override void RemoveItem(object item)
        {
            DestroyItem((ComponentBoxItemView)item);

            base.RemoveItem(item);
        }

        public override void AddItem(object item)
        {
            InitializeItem((ComponentBoxItemView)item);

            base.AddItem(item);
        }

        private void DestroyItem(ComponentBoxItemView item)
        {
            // TODO: Dispose
        }

        private void InitializeItem(ComponentBoxItemView item)
        {
            // TODO: Init
        }
    }
}
