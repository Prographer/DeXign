using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;

[assembly: ExportRenderer(typeof(PFunction), typeof(Function), typeof(FunctionRenderer))]

namespace DeXign.Editor.Renderer
{
    public class FunctionRenderer : ComponentRenderer<PFunction, Function>
    {
        public FunctionRenderer(Function adornedElement, PFunction model) : base(adornedElement, model)
        {
        }
    }
}