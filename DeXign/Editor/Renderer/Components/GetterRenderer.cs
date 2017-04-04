using System.Windows;
using System.Windows.Media;

using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;

using WPFExtension;

[assembly: ExportRenderer(typeof(PGetter), typeof(Getter), typeof(GetterRenderer))]

namespace DeXign.Editor.Renderer
{
    public class GetterRenderer : ComponentRenderer<PGetter, Getter>
    {
        public GetterRenderer(Getter adornedElement, PGetter model) : base(adornedElement, model)
        {
        }

        protected override void OnLoaded(FrameworkElement adornedElement)
        {
            base.OnLoaded(adornedElement);

            BindingHelper.SetBinding(
                Model, PGetter.TargetTypeProperty,
                Element, Getter.TargetTypeProperty);

            BindingHelper.SetBinding(
                Model, PGetter.PropertyProperty,
                Element, Getter.SelectedPropertyProperty);
        }

        protected override void OnDrawOutSightText(DrawingContext drawingContext)
        {
            DrawSightText(drawingContext, "GET");
        }

    }
}
