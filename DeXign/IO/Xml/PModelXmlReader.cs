using System.IO;
using System.Xaml;

using DeXign.Core;

namespace DeXign.IO
{
    public class PModelXmlReader : StreamReader
    {
        public PModelXmlReader(Stream stream) : base(stream)
        {
        }

        public PObject ReadModel()
        {
            this.BaseStream.Seek(0, SeekOrigin.Begin);

            object obj = XamlServices.Parse(this.ReadToEnd());

            if (obj is PObject)
                return (PObject)obj;

            return null;
        }
    }
}
