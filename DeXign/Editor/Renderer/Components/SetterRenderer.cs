using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;

[assembly: ExportRenderer(typeof(PSetter), typeof(Setter), typeof(SetterRenderer))]

namespace DeXign.Editor.Renderer
{
    public class SetterRenderer : ComponentRenderer<PSetter, Setter>
    {
        public SetterRenderer(Setter adornedElement, PSetter model) : base(adornedElement, model)
        {
        }
    }
}
