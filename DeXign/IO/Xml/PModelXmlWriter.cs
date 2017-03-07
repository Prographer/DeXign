using System.IO;
using System.Xml;
using System.Xaml;

using DeXign.Core;
using System;

namespace DeXign.IO
{
    public class PModelXmlWriter : StreamWriter
    {
        public PModelXmlWriter(Stream stream) : base(stream)
        {
        }

        public void WriteModel(PObject model)
        {
            var settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            this.BaseStream.Seek(0, SeekOrigin.Begin);

            var writer = XmlWriter.Create(this.BaseStream, settings);

            XamlServices.Save(writer, model);
        }
    }
}
