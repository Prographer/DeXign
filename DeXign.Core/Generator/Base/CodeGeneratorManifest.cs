using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeXign.Core
{
    public class CodeGeneratorManifest
    {
        public string Version { get; set; } = "1.0.0";

        public string PackageName { get; set; } = "com.dexign";

        public string NamespaceName { get; set; } = "DeXign";
    }
}
