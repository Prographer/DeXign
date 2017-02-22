using System.Collections.Generic;

namespace DeXign.Editor
{
    interface IGuideService
    {
        double SnapThreshold { get; set; }

        IEnumerable<Guideline> GetSnappedGuidelines(IGuideProvider provider);
    }
}
