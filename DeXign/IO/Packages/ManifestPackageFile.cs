using System.IO;
using System.Xml.Serialization;

namespace DeXign.IO
{
    internal class ManifestPackageFile : PackageFile
    {
        public const string FileName = "Manifest.xml";

        public ManifestPackageFile(DXProjectManifest manifest)
        {
            var xmlStream = new MemoryStream();
            var xmlSerializer = new XmlSerializer(typeof(DXProjectManifest));

            xmlSerializer.Serialize(xmlStream, manifest);

            this.Stream = xmlStream;
            this.Name = ManifestPackageFile.FileName;
        }
    }
}