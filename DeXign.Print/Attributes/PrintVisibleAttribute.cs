using System;

namespace DeXign.Core
{
    public class PrintVisibleAttribute : Attribute
    {
        public bool Visible { get; set; } = true;
        public string DisplayName { get; set; }

        public PrintVisibleAttribute(bool visible)
        {
            this.Visible = visible;
        }

        public PrintVisibleAttribute(bool visible, string displayName)
        {
            this.Visible = visible;
            this.DisplayName = displayName;
        }

        public PrintVisibleAttribute(string displayName)
        {
            this.DisplayName = displayName;
        }
    }
}
