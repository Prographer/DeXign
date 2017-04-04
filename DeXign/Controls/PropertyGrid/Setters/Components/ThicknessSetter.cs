using DeXign.Extension;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFExtension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_leftValueBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_topValueBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_rightValueBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_bottomValueBox", Type = typeof(TextBox))]
    [Setter(Type = typeof(Thickness))]
    class ThicknessSetter : BaseSetter
    {
        public static readonly DependencyProperty IsMarginProperty =
            DependencyHelper.Register();

        public bool IsMargin
        {
            get { return this.GetValue<bool>(IsMarginProperty); }
            set { SetValue(IsMarginProperty, value); }
        }

        TextBox leftValueBox;
        TextBox topValueBox;
        TextBox rightValueBox;
        TextBox bottomValueBox;

        ElementThicknessBinder marginBinder;

        public ThicknessSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
            marginBinder = new ElementThicknessBinder(Target, TargetDependencyProperty);
            IsMargin = pi.Name == "Margin";
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            leftValueBox = GetTemplateChild<TextBox>("PART_leftValueBox");
            topValueBox = GetTemplateChild<TextBox>("PART_topValueBox");
            rightValueBox = GetTemplateChild<TextBox>("PART_rightValueBox");
            bottomValueBox = GetTemplateChild<TextBox>("PART_bottomValueBox");
            
            BindingHelper.SetBinding(
                marginBinder, ElementThicknessBinder.LeftProperty,
                leftValueBox, TextBox.TextProperty);

            BindingHelper.SetBinding(
                marginBinder, ElementThicknessBinder.TopProperty,
                topValueBox, TextBox.TextProperty);

            BindingHelper.SetBinding(
                marginBinder, ElementThicknessBinder.RightProperty,
                rightValueBox, TextBox.TextProperty);

            BindingHelper.SetBinding(
                marginBinder, ElementThicknessBinder.BottomProperty,
                bottomValueBox, TextBox.TextProperty);
        }

        protected override void OnDispose()
        {
            leftValueBox = null;
            topValueBox = null;
            rightValueBox = null;
            bottomValueBox = null;
        }
    }
}
