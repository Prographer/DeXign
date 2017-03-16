using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;
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

        public IEnumerable<LineConnector> FromThumb(BindThumb outputThumb, BindThumb inputThumb)
        {
            return this
                .Where(lc => lc is LineConnector)
                .Select(lc => (LineConnector)lc)
                .Where(lc => lc.Output.Equals(outputThumb) && lc.Input.Equals(inputThumb));
        }

        public bool HasThumbExpression(BindThumb outputThumb, BindThumb inputThumb)
        {
            return this.FromThumb(outputThumb, inputThumb).Count() > 0;
        }
    }
}
