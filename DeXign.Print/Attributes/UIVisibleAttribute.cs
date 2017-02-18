using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeXign.Core
{
    public class UIVisibleAttribute : Attribute
    {
        public bool Visible { get; set; } = true;
        public string DisplayName { get; set; }

        public UIVisibleAttribute(bool visible)
        {
            this.Visible = visible;
        }

        public UIVisibleAttribute(bool visible, string displayName)
        {
            this.Visible = visible;
            this.DisplayName = displayName;
        }

        public UIVisibleAttribute(string displayName)
        {
            this.DisplayName = displayName;
        }
    }
}
