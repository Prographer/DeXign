using System;

namespace DeXign.Core
{
    public class DesignElement : Attribute
    {
        public bool Visible { get; set; } = true;

        public string DisplayName { get; set; }

        public DesignElement()
        {
        }
    }
}
