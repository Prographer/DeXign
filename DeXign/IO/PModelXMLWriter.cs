using System.IO;

using DeXign.Core;

namespace DeXign.IO
{
    class PModelXMLWriter : StreamWriter
    {
        public PModelXMLWriter(Stream stream) : base(stream)
        {
        }

        public void WriteModel(PObject model)
        {
            // ~~~~~ Write ~~~~~~
        }
    }
}
