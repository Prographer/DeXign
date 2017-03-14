
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Core.Logic.Component;

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