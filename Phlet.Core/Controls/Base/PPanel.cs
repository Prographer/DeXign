using Phlet.Core.Collections;

namespace Phlet.Core.Controls
{
    public class PPanel : PControl
    {
        private PControlCollection _pControlCollection;
        public PControlCollection Children
        {
            get
            {
                if (_pControlCollection == null)
                    _pControlCollection = new PControlCollection();
                
                return _pControlCollection;
            }
        }
    }
}