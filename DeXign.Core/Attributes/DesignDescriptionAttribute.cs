using System;

namespace DeXign.Core
{
    public class DesignDescriptionAttribute : Attribute
    {
        public string Description { get; set; }

        public DesignDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}
