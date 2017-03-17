using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;
using DeXign.Extension;

using w = System.Windows;

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

            BindingEx.SetBinding(
                Model, PSetter.TargetTypeProperty,
                Element, Setter.TargetTypeProperty);

            BindingEx.SetBinding(
                Model, PSetter.PropertyProperty,
                Element, Setter.SelectedPropertyProperty);
        }
    }
}
