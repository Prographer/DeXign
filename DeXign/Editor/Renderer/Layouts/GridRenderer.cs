using DeXign.Core.Controls;
using System;
using System.Windows.Controls;
using System.Windows;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Core;
using DeXign.Core.Designer;
using System.Collections.Specialized;
using System.Collections;
using DeXign.Input;

[assembly: ExportRenderer(typeof(PGridLayout), typeof(Grid), typeof(GridRenderer))]

namespace DeXign.Editor.Renderer
{
    class GridRenderer : LayerRenderer<PGridLayout, Grid>, IGridLayout
    {
        public GridRenderer(Grid adornedElement, PGridLayout model) : base(adornedElement, model)
        {
        }

        protected override void OnElementAttached(Grid element)
        {
            base.OnElementAttached(element);

            if (!IsContentParent())
                this.SetSize(100, 100);

            // Binding
            Model.ColumnDefinitions.CollectionChanged += 
                (s, e) => DefinitionsCollectionChanged<PColumnDefinition, ColumnDefinition>(Element.ColumnDefinitions, e);

            Model.RowDefinitions.CollectionChanged += 
                (s, e) => DefinitionsCollectionChanged<PRowDefinition, RowDefinition>(Element.RowDefinitions, e);

            // TODO: ColumnSpacing
            // TODO: RowSpacing
        }

        private void DefinitionsCollectionChanged<PDefinition, NDefinition>(IList list, NotifyCollectionChangedEventArgs e)
            where PDefinition : PObject, IDefinition
            where NDefinition : DefinitionBase
        {
            for (int i = e.OldStartingIndex; i < e.OldStartingIndex + e.OldItems?.Count; i++)
            {
                var definition = (PDefinition)e.OldItems[i - e.OldStartingIndex];
                var wpfDefinition = (NDefinition)definition.GetDesignTag();

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        list.Remove(wpfDefinition);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        list[i] = definition;
                        break;
                }
            }

            for (int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems?.Count; i++)
            {
                var definition = e.NewItems[i - e.NewStartingIndex] as PDefinition;
                var wpfDefinition = Convert.ChangeType(definition, typeof(NDefinition));

                definition.SetDesignTag(wpfDefinition);

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        list.Add(wpfDefinition);
                        break;
                }
            }
        }

        public override bool CanDrop(AttributeTuple<DesignElementAttribute, Type> item)
        {
            return item != null;
        }

        public override void OnAddedChild(IRenderer child)
        {
            var position = this.PointFromScreen(SystemMouse.Position);

            child.Element.Width = 100;
            child.Element.Height = 100;

            child.Element.Margin = new Thickness(position.X, position.Y, 0, 0);
            child.Element.HorizontalAlignment = HorizontalAlignment.Left;
            child.Element.VerticalAlignment = VerticalAlignment.Top;
        }
    }
}
