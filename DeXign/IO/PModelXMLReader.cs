using System;
using System.IO;

using DeXign.Core;

namespace DeXign.IO
{
    class PModelXMLReader : StreamReader
    {
        public PModelXMLReader(Stream stream) : base(stream)
        {
        }

        public PObject ReadModel()
        {
            throw new NotImplementedException();
        }
    }
}
