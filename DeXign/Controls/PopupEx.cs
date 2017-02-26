using DeXign.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DeXign.Controls
{
    class PopupEx : Popup
    {
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            AlignCenter();
        }
        
        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            AlignCenter();
        }

        private void AlignCenter()
        {
            var parent = GetUIParentCore() as FrameworkElement;
            var child = Child as FrameworkElement;

            if (Placement == PlacementMode.Left || Placement == PlacementMode.Right)
            {
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                        VerticalOffset = -(child.DesiredSize.Height - parent.DesiredSize.Height) / 2;
                        break;

                    case VerticalAlignment.Bottom:
                        VerticalOffset = -(child.DesiredSize.Height - parent.DesiredSize.Height);
                        break;
                }
            }

            if (Placement == PlacementMode.Top || Placement == PlacementMode.Bottom)
            {
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        HorizontalOffset = -(child.DesiredSize.Width - parent.DesiredSize.Width) / 2;
                        break;

                    case HorizontalAlignment.Right:
                        HorizontalOffset = -(child.DesiredSize.Width - parent.DesiredSize.Width);
                        break;
                }
            }
        }
    }
}
