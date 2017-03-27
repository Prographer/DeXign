using DeXign.Core.Controls;

namespace DeXign.Core.Logic
{
    public class PLayoutBinderHost : PBinderHost
    {
        public PVisual LogicalParent { get; set; }

        public PLayoutBinderHost()
        {
        }

        public PLayoutBinderHost(PVisual visual)
        {
            this.LogicalParent = visual;
        }
    }
}
