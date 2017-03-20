using System.Linq;
using System.Collections.Generic;

using DeXign.Core;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Core.Logic;

namespace DeXign.IO
{
    internal class ComponentRendererPackageFile : ModelPackageFile<ObjectContainer<RendererSurface>>
    {
        public const string Path = "Renderers";
        public const string FileName = "Renderers\\Components.xml";

        public ComponentRendererPackageFile(List<PComponent> list)
        {
            this.Name = FileName;

            List<IRenderer> childrens = list
                .Select(c => c.GetRenderer())
                .ToList();

            this.Model = new ObjectContainer<RendererSurface>()
            {
                Items = childrens
                    .Select(c => new RendererSurface()
                    {
                        Guid = c.Model.Guid,
                        Metadata = c.Metadata
                    }).ToList()
            };

            base.CreateStream();
        }
    }
}
