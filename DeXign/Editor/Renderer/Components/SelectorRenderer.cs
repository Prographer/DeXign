using System.Windows;
using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;
using System.Windows.Media;
using WPFExtension;
using System;
using DeXign.Utilities;
using DeXign.Core.Controls;
using DeXign.Editor.Layer;

[assembly: ExportRenderer(typeof(PSelector), typeof(ObjectSelector), typeof(SelectorRenderer))]

namespace DeXign.Editor.Renderer
{
    public class SelectorRenderer : ComponentRenderer<PSelector, ObjectSelector>
    {
        public SelectorRenderer(ObjectSelector adornedElement, PSelector model) : base(adornedElement, model)
        {
            if (this.Model.TargetVisual != null)
            {
                var registedModel = GlobalModels.GetModel<PVisual>(this.Model.TargetVisual.Guid);

                this.Model.TargetVisual = registedModel;
            }
        }

        protected override void OnLoaded(FrameworkElement adornedElement)
        {
            base.OnLoaded(adornedElement);
        }

        protected override void OnDrawOutSightText(DrawingContext drawingContext)
        {
            // Prevent Text Draw
        }

        protected override void OnSelected()
        {
            if (this.Element.TargetRenderer is SelectionLayer layer)
            {
                layer.IsHighlight = true;
            }
        }

        protected override void OnUnSelected()
        {
            if (this.Element.TargetRenderer is SelectionLayer layer)
            {
                layer.IsHighlight = false;
            }
        }
    }
}
