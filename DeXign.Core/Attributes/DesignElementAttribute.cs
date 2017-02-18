using System;

namespace DeXign.Core
{
    public class DesignElementAttribute : Attribute
    {
        public bool Visible { get; set; } = true;

        public string DisplayName { get; set; }

        public string Category { get; set; }
    }
}
