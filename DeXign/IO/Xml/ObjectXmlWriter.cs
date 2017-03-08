using System.IO;
using System.Xml;
using System.Xaml;

namespace DeXign.IO
{
    public class ObjectXmlWriter : StreamWriter
    {
        public ObjectXmlWriter(Stream stream) : base(stream)
        {
        }

        public void WriteObject(object obj)
        {
            var settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            this.BaseStream.Seek(0, SeekOrigin.Begin);

            var writer = XmlWriter.Create(this.BaseStream, settings);

            XamlServices.Save(writer, obj);
        }
    }
}
