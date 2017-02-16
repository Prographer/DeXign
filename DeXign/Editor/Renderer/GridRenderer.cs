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

[assembly: ExportRenderer(typeof(PGridLayout), typeof(Grid), typeof(GridRenderer))]

namespace DeXign.Editor.Renderer
{
    class GridRenderer : LayerRenderer<PGridLayout, Grid>
    {
        public GridRenderer(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnElementAttached(Grid element)
        {
            base.OnElementAttached(element);

            if (!IsContentParent())
            {
                element.Width = 100;
                element.Height = 100;
            }

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
    }
}
