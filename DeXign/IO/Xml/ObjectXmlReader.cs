using System.IO;
using System.Xaml;

namespace DeXign.IO
{
    public class ObjectXmlReader : StreamReader
    {
        public ObjectXmlReader(Stream stream) : base(stream)
        {
        }

        public object ReadObject()
        {
            this.BaseStream.Seek(0, SeekOrigin.Begin);

            return XamlServices.Parse(this.ReadToEnd());
        }
    }
}
