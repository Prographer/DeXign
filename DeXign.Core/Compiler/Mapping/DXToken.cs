using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeXign.Core.Compiler
{
    public struct DXToken
    {
        const string BlockPattern = @"{.+?}";
        const string Pattern = @"{(?:\<(?<Safe>Safe)\>)?(?<Key>.+?)(?::(?<Param>.+?))?}";

        public string OriginalSource { get; }

        public string Token { get; }

        public string Parameter { get; }

        public bool IsSafe { get; }

        public bool IsIndexed => int.TryParse(this.Parameter, out int r);
        
        public bool HasParameter => !string.IsNullOrWhiteSpace(this.Parameter);

        public bool HasReturn => this.Parameter == "return;";

        public DXToken(string source)
        {
            this.OriginalSource = source;

            Match m = Regex.Match(source, Pattern);

            if (!m.Success)
                throw new ArgumentException();

            this.IsSafe = (m.Groups["Safe"].Value == "Safe");

            this.Token = m.Groups["Key"].Value.Trim();
            this.Parameter = m.Groups["Param"].Value?.Trim();
        }

        public static IEnumerable<DXToken> Parse(string source)
        {
            return Regex.Matches(source, BlockPattern)
                .OfType<Match>()
                .Select(m => new DXToken(m.Value))
                .Distinct();
        }

        public override string ToString()
        {
            return this.OriginalSource;
        }
    }
}
