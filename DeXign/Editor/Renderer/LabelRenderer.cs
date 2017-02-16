using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Core.Controls;
using System.Windows;
using DeXign.Extension;

[assembly: ExportRenderer(typeof(PLabel), typeof(TextBlock), typeof(LabelRenderer))]

namespace DeXign.Editor.Renderer
{
    class LabelRenderer : LayerRenderer<PLabel, TextBlock>
    {
        public LabelRenderer(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnElementAttached(TextBlock element)
        {
            base.OnElementAttached(element);

            BindingEx.SetBinding(
                Model, PLabel.TextProperty,
                element, TextBlock.TextProperty);

            Model.Text = "텍스트";
        }
    }
}
