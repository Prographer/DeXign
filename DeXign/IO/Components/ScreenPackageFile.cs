using DeXign.Core;
using DeXign.Core.Controls;
using System.IO;

namespace DeXign.IO
{
    internal class ScreenPackageFile : PackageFile
    {
        public ScreenPackageFile(PContentPage screen)
        {
            var xmlStream = new MemoryStream();
            var xmlWriter = new PModelXMLWriter(xmlStream);

            xmlWriter.WriteModel(screen);

            this.Name = LayoutExtension.GetPageName(screen);
            this.Stream = xmlStream;
        }
    }
}
