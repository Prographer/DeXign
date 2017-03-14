using DeXign.Editor;
using System.Collections.Generic;
using System.Linq;

namespace DeXign.Utilities
{
    class LineConnectorCollection : List<LineConnectorBase>
    {
        public LineConnectorCollection()
        {
        }

        public LineConnectorCollection(int capacity) : base(capacity)
        {
        }

        public LineConnectorCollection(IEnumerable<LineConnectorBase> collection) : base(collection)
        {
        }

        public IEnumerable<LineConnector> FromRenderer(IRenderer outputRenderer, IRenderer inputRenderer)
        {
            return this
                .Where(lc => lc is LineConnector)
                .Select(lc => (LineConnector)lc)
                .Where(lc => lc.Output.Renderer.Equals(outputRenderer) && lc.Input.Renderer.Equals(inputRenderer));
        }
    }
}
