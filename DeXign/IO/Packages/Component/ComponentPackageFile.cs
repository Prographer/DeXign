using DeXign.Core.Logic;
using System.Collections.Generic;

namespace DeXign.IO
{
    internal class ComponentPackageFile : ModelPackageFile<ObjectContainer<PComponent>>
    {
        public const string Path = "Component";
        public const string FileName = "Component/Components.xml";

        public ComponentPackageFile(List<PComponent> model)
        {
            this.Name = FileName;

            this.Model = new ObjectContainer<PComponent>()
            {
                Items = model
            };

            base.CreateStream();
        }
    }
}
