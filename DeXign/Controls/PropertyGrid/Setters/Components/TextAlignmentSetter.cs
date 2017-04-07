using System.Windows;
using System.Reflection;

using DeXign.Core;

namespace DeXign.Controls
{
    [Setter(Type = typeof(PVerticalTextAlignment))]
    class VerticalTextAlignmentSetter : EnumRadioSetter
    {
        public VerticalTextAlignmentSetter(DependencyObject[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }

    [Setter(Type = typeof(PHorizontalTextAlignment))]
    class HorizontalTextAlignmentSetter : EnumRadioSetter
    {
        public HorizontalTextAlignmentSetter(DependencyObject[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }
    }
}
