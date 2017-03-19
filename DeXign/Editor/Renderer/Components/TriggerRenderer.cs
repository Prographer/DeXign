using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Renderer;

using l = DeXign.Editor.Logic;

[assembly: ExportRenderer(typeof(PTrigger), typeof(l.Trigger), typeof(TriggerRenderer))]

namespace DeXign.Editor.Renderer
{
    public class TriggerRenderer : ComponentRenderer<PTrigger, l.Trigger>
    {
        public TriggerRenderer(l.Trigger adornedElement, PTrigger model) : base(adornedElement, model)
        {
        }
    }
}