using System;

namespace DeXign.IO
{
    public class DXReferencedModuleCollection : ObjectContainer<string>
    {
    }

    [Serializable]
    public class DXProjectManifest
    {
        public string ProjectName { get; set; }
        public string PackageName { get; set; }

        public DXReferencedModuleCollection ReferencedModules { get; set; }

        public DXProjectManifest()
        {
            ReferencedModules = new DXReferencedModuleCollection();
        }
    }
}
