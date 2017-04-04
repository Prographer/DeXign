using System.Windows.Media;
using w = System.Windows;

using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;

using WPFExtension;

[assembly: ExportRenderer(typeof(PSetter), typeof(Setter), typeof(SetterRenderer))]

namespace DeXign.Editor.Renderer
{
    public class SetterRenderer : ComponentRenderer<PSetter, Setter>
    {
        public SetterRenderer(Setter adornedElement, PSetter model) : base(adornedElement, model)
        {
        }

        protected override void OnLoaded(w.FrameworkElement adornedElement)
        {
            base.OnLoaded(adornedElement);

            BindingHelper.SetBinding(
                Model, PSetter.TargetTypeProperty,
                Element, Setter.TargetTypeProperty);

            BindingHelper.SetBinding(
                Model, PSetter.PropertyProperty,
                Element, Setter.SelectedPropertyProperty);
        }

        protected override void OnDrawOutSightText(DrawingContext drawingContext)
        {
            DrawSightText(drawingContext, "SET");
        }
    }
}
