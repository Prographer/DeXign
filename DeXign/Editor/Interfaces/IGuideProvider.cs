using System.Collections.Generic;

namespace DeXign.Editor
{
    interface IGuideProvider
    {
        IEnumerable<Guideline> GetGuidableLines();
    }
}
