using System.Collections.Generic;

namespace DeXign.Core.Compiler
{
    public class DXCompileResult
    {
        public DXCompileOption Option { get; }

        public bool IsSuccess { get; set; } = false;

        public List<string> Outputs { get; }

        public List<object> Errors { get; }

        public DXCompileResult(DXCompileOption option)
        {
            this.Option = option;

            this.Outputs = new List<string>();
            this.Errors = new List<object>();
        }
    }
}
