using System;

namespace DeXign.Designer
{
    class ExportRendererAttribute : Attribute
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
