using System;
using DeXign.Editor.Renderer;
using System.Windows;

namespace DeXign.IO
{
    public class RendererSurface
    {
        public Guid Guid { get; set; }
        public RendererMetadata Metadata { get; set; }
        public Point Location { get; set; }
    }
}
