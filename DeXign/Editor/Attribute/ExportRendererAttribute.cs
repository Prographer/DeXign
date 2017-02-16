using System;

namespace DeXign.Editor
{
    // Example
    //
    // [assembly: ExportRenderer(typeof(<PObject>), typeof(<FrameworkElement>), typeof(<LayerRenderer<PObject, FrameworkElement>>))]

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ExportRendererAttribute : Attribute
    {
        public Type ModelType { get; set; }    // PObject ~
        public Type ViewType { get; set; }     // WPF Object ~
        public Type RendererType { get; set; } // Layer

        public ExportRendererAttribute(Type modelType, Type viewType, Type rendererType)
        {
            this.ModelType = modelType;
            this.ViewType = viewType;
            this.RendererType = rendererType;
        }
    }
}
