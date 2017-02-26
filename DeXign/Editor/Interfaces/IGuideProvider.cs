using System.Collections.Generic;

namespace DeXign.Editor
{
    public interface IGuideProvider
    {
        IEnumerable<Guideline> GetGuidableLines();
    }
}
