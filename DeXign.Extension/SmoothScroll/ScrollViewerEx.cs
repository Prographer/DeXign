using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DeXign.Extension
{
    public static class ScrollViewerEx
    {
        static Dictionary<UIElement, SmoothScrollBehavior> behaviors 
            = new Dictionary<UIElement, SmoothScrollBehavior>();

        static Dictionary<UIElement, double> stepSizes
            = new Dictionary<UIElement, double>();

        public static void SetSmoothScrollStepSize(this UIElement ui, double stepSize)
        {
            stepSizes[ui] = stepSize;

            if (behaviors.ContainsKey(ui))
                behaviors[ui].StepSize = stepSize;
        }

        public static void SetSmoothScrollEnabled(this UIElement ui, bool isSmooth)
        {
            if (isSmooth)
            {
                if (!behaviors.ContainsKey(ui))
                {
                    ScrollViewer target = null;

                    target = ui.FindChildrens<ScrollViewer>().FirstOrDefault();

                    if (target != null)
                    {
                        behaviors[ui] = new SmoothScrollBehavior(ui, target);

                        if (stepSizes.ContainsKey(ui))
                            behaviors[ui].StepSize = stepSizes[ui];
                        else
                            stepSizes[ui] = behaviors[ui].StepSize;
                    }
                }
            }
            else
            {
                if (behaviors.ContainsKey(ui))
                {
                    behaviors[ui].Dispose();
                    behaviors.Remove(ui);
                }
            }
        }
    }
}
