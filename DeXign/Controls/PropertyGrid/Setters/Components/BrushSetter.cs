using System.Windows;
using System.Windows.Media;
using System.Reflection;

using Xceed.Wpf.Toolkit;
using DeXign.Extension;
using WPFExtension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_colorCanvas", Type = typeof(ColorCanvas))]
    [Setter(Type = typeof(Brush))]
    class BrushSetter : BaseSetter
    {
        ColorCanvas colorCanvas;

        public BrushSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            colorCanvas = GetTemplateChild<ColorCanvas>("PART_colorCanvas");
            
            if (this.Value is SolidColorBrush)
            {
                var brush = (SolidColorBrush)this.Value;

                colorCanvas.SelectedColor = brush.Color;

                TargetDependencyProperty.AddValueChanged(Target, Brush_Changed);
                colorCanvas.SelectedColorChanged += ColorCanvas_SelectedColorChanged;
            }
            else
            {
                this.IsEnabled = false;
            }
        }

        private void ColorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Value = new SolidColorBrush(e.NewValue.Value);
        }

        private void Brush_Changed(object sender, System.EventArgs e)
        {
            var brush = (Value as SolidColorBrush);

            if (colorCanvas != null && !brush.Color.Equals(colorCanvas.SelectedColor))
                colorCanvas.SelectedColor = brush.Color;
        }

        protected override void OnDispose()
        {
            if (this.Value is SolidColorBrush)
            {
                TargetDependencyProperty.RemoveValueChanged(Target, Brush_Changed);
                colorCanvas.SelectedColorChanged -= ColorCanvas_SelectedColorChanged;

                colorCanvas = null;
            }
        }
    }
}
