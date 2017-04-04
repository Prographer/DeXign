using System;
using System.Windows;
using WPFExtension;

namespace DeXign
{
    class ElementThicknessBinder : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty LeftProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty TopProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty RightProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty BottomProperty =
            DependencyHelper.Register();

        public double Left
        {
            get { return this.GetValue<double>(LeftProperty); }
            set { this.SetValue(LeftProperty, value); }
        }

        public double Top
        {
            get { return this.GetValue<double>(TopProperty); }
            set { this.SetValue(TopProperty, value); }
        }

        public double Right
        {
            get { return this.GetValue<double>(RightProperty); }
            set { this.SetValue(RightProperty, value); }
        }

        public double Bottom
        {
            get { return this.GetValue<double>(BottomProperty); }
            set { this.SetValue(BottomProperty, value); }
        }

        public DependencyObject Target { get; private set; }

        public DependencyProperty TargetThicknessProperty { get; private set; }

        public ElementThicknessBinder(DependencyObject obj, DependencyProperty thicknessDepedencyProperty)
        {
            this.Target = obj;
            this.TargetThicknessProperty = thicknessDepedencyProperty;

            UpdateValues();

            LeftProperty.AddValueChanged(this, Value_Changed);
            TopProperty.AddValueChanged(this, Value_Changed);
            RightProperty.AddValueChanged(this, Value_Changed);
            BottomProperty.AddValueChanged(this, Value_Changed);

            thicknessDepedencyProperty.AddValueChanged(obj, Thickness_Changed);
        }

        private void Thickness_Changed(object sender, EventArgs e)
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            var value = (Thickness)Target.GetValue(TargetThicknessProperty);

            if (Left != value.Left)
                Left = value.Left;

            if (Top != value.Top)
                Top = value.Top;

            if (Right != value.Right)
                Right = value.Right;

            if (Bottom != value.Bottom)
                Bottom = value.Bottom;
        }

        private void Value_Changed(object sender, EventArgs e)
        {
            var value = (Thickness)Target.GetValue(TargetThicknessProperty);

            value.Left = Left;
            value.Top = Top;
            value.Right = Right;
            value.Bottom = Bottom;

            Target.SetValue(TargetThicknessProperty, value);
        }

        public void Dispose()
        {
            if (Target != null)
            {
                LeftProperty.RemoveValueChanged(this, Value_Changed);
                TopProperty.RemoveValueChanged(this, Value_Changed);
                RightProperty.RemoveValueChanged(this, Value_Changed);
                BottomProperty.RemoveValueChanged(this, Value_Changed);

                TargetThicknessProperty.RemoveValueChanged(Target, Thickness_Changed);
            }

            Target = null;
            TargetThicknessProperty = null;
        }
    }
}
