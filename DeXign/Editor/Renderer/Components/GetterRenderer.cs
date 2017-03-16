using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using System.Windows;

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

            BindingEx.SetBinding(
                Model, PGetter.TargetTypeProperty,
                Element, Getter.TargetTypeProperty);

            BindingEx.SetBinding(
                Model, PGetter.PropertyProperty,
                Element, Getter.SelectedPropertyProperty);
        }
    }
}
