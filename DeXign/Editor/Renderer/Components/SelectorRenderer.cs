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
    }
}
