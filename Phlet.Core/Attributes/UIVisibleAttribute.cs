using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phlet.Core
{
    public class UIVisibleAttribute : Attribute
    {
        public bool Visible { get; set; } = true;
        public string DisplayName { get; set; }

        public UIVisible(bool visible)
        {
            this.Visible = visible;
        }

        public UIVisible(bool visible, string displayName)
        {
            this.Visible = visible;
            this.DisplayName = displayName;
        }

        public UIVisible(string displayName)
        {
            this.DisplayName = displayName;
        }
    }
}
