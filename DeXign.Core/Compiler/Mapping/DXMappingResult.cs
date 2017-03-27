using DeXign.Extension;
using System.Collections.Generic;

namespace DeXign.Core.Compiler
{
    public class DXMappingResult
    {
        public string Source { get; set; }

        public List<DXToken> Resolved { get; }

        public List<DXToken> Errors { get; }

        public DXMappingResult()
        {
            this.Resolved = new List<DXToken>();
            this.Errors = new List<DXToken>();
        }

        public void AddResolvedToken(DXToken token)
        {
            this.Errors.SafeRemove(token);
            this.Resolved.SafeAdd(token);
        }

        public void AddErrorToken(DXToken token)
        {
            this.Resolved.SafeRemove(token);
            this.Errors.SafeAdd(token);
        }
    }
}
