﻿using DeXign.Extension;
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
            get { return (bool)GetValue(IsMarginProperty); }
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

            leftValueBox.KeyDown += ValueBox_KeyDown;
            topValueBox.KeyDown += ValueBox_KeyDown;
            rightValueBox.KeyDown += ValueBox_KeyDown;
            bottomValueBox.KeyDown += ValueBox_KeyDown;

            BindingEx.SetBinding(
                marginBinder, ElementThicknessBinder.LeftProperty,
                leftValueBox, TextBox.TextProperty);

            BindingEx.SetBinding(
                marginBinder, ElementThicknessBinder.TopProperty,
                topValueBox, TextBox.TextProperty);

            BindingEx.SetBinding(
                marginBinder, ElementThicknessBinder.RightProperty,
                rightValueBox, TextBox.TextProperty);

            BindingEx.SetBinding(
                marginBinder, ElementThicknessBinder.BottomProperty,
                bottomValueBox, TextBox.TextProperty);
        }

        private void ValueBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (sender as TextBox)
                .GetBindingExpression(TextBox.TextProperty)
                .UpdateSource();
            }
        }
    }
}
