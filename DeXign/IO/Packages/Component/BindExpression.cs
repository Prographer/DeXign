using System;

namespace DeXign.IO
{
    public struct BindExpression
    {
        public Guid Output { get; set; }
        public Guid Input { get; set; }

        public override int GetHashCode()
        {
            return $"{Output.GetHashCode()}{Input.GetHashCode()}".GetHashCode();
        }
    }
}
