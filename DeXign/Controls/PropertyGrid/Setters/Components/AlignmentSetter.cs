using System.Windows;
using System.Reflection;

using DeXign.Core;

namespace DeXign.Controls
{
    [Setter(Type = typeof(PVerticalAlignment))]
    class VerticalAlignmentSetter : EnumRadioSetter
    {
        public VerticalAlignmentSetter(DependencyObject[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }

    [Setter(Type = typeof(PHorizontalAlignment))]
    class HorizontalAlignmentSetter : EnumRadioSetter
    {
        public HorizontalAlignmentSetter(DependencyObject[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }
    }
}
