using DeXign.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeXign.Editor.Renderer
{
    class ScrollViewRenderer : LayerRenderer<PScrollView, ScrollViewer>
    {
        public ScrollViewRenderer(ScrollViewer adornedElement, PScrollView model) : base(adornedElement, model)
        {
        }
    }
}
