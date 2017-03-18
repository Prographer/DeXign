using System;
using System.Windows;

using DeXign.Controls;
using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Models;

using WPFExtension;
using DeXign.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DeXign.Editor.Renderer;
using DeXign.Editor.Controls;
using DeXign.Core.Controls;
using DeXign.Core.Logic;
using DeXign.Extension;

namespace DeXign.Editor.Logic
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

        public object TargetObject
        {
            get { return GetValue(TargetObjectProperty); }
            set
            {
                SetValue(TargetObjectProperty, value);

                InvlidateTargetObject();
            }
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
        
        public ComponentBox()
        {
        }

        protected override void OnSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = SelectedItem as ComponentBoxItemView;

            if (item == null)
                return;

            ItemSelected?.Invoke(this, item.Model);

            this.UnselectAll();
        }

        private void InvlidateTargetObject()
        {
            this.Clear();

            if (TargetObject != null)
            {
                if (TargetObject is PObject)
                    AddEventItems(TargetObject.GetType());

                if (TargetObject is Storyboard)
                {
                    AddComponentItems(
                        DesignerManager.GetElementTypes()
                            .Where(attr => attr.Attribute.Visible && attr.Element.CanCastingTo<PComponent>()));

                    AddRendererItems(
                        GlobalModels.Items
                            .Select(obj => obj.GetRenderer())
                            .Where(r =>
                            {
                                if (r == null)
                                    return false;

                                // 렌더러가 태스크에 의해 삭제된 상태 (파괴되기전)
                                if (!r.Element.IsVisible)
                                    return false;
                                
                                if (r.Model is PVisual == false)
                                    return false;

                                return true;
                            }));
                }
            }

            IsEmpty = this.ItemCount == 0;
        }

        private void AddComponentItems(IEnumerable<AttributeTuple<DesignElementAttribute, Type>> componentTypes)
        {
            foreach (var attr in componentTypes)
            {
                var item = new ComponentBoxItemView(
                    new ComponentBoxItemModel(attr));

                this.AddItem(item);
            }
        }

        private void AddRendererItems(IEnumerable<IRenderer> renderers)
        {
            foreach (IRenderer renderer in renderers)
            {
                var item = new ComponentBoxItemView(
                    new ComponentBoxItemModel(renderer));

                this.AddItem(item);
            }
        }

        private void AddEventItems(Type pObjType)
        {
            foreach (var ev in DesignerManager.GetEvents(pObjType))
            {
                var item = new ComponentBoxItemView(
                    new ComponentBoxItemModel(ev));

                this.AddItem(item);
            }
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
