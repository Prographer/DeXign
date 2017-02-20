using System.Windows;
using System.Reflection;

using DeXign.Core;

namespace DeXign.Controls
{
    [Setter(Type = typeof(PVerticalTextAlignment))]
    class VerticalTextAlignmentSetter : EnumRadioSetter
    {
        public VerticalTextAlignmentSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
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
        public HorizontalTextAlignmentSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }
    }
}
