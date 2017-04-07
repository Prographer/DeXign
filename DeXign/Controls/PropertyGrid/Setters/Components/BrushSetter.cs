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

        public BrushSetter(DependencyObject[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //colorCanvas = GetTemplateChild<ColorCanvas>("PART_colorCanvas");
            
            //if (this.Value.PropertyType == typeof(SolidColorBrush))
            //{
            //    if (this.Value.IsStable)
            //    {
            //        colorCanvas.SelectedColor = (this.Value.Value as SolidColorBrush).Color;
            //    }
            //    else
            //    {
            //        colorCanvas.SelectedColor = Colors.Transparent;
            //    }

            //    // hook
            //    for (int i = 0; i < Targets.Length; i++)
            //    {
            //        TargetDependencyProperties[i].AddValueChanged(Targets[i], Brush_Changed);
            //    }
                
            //    colorCanvas.SelectedColorChanged += ColorCanvas_SelectedColorChanged;
            //}
            //else
            //{
            //    this.IsEnabled = false;
            //}
        }

        private void ColorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            //Value = new PropertyValue(new SolidColorBrush(e.NewValue.Value));
        }

        private void Brush_Changed(object sender, System.EventArgs e)
        {
            //var brush = (Value as SolidColorBrush);

            //if (colorCanvas != null && 
            //    brush != null && !brush.Color.Equals(colorCanvas.SelectedColor))
            //    colorCanvas.SelectedColor = brush.Color;
        }

        protected override void OnDispose()
        {
            //if (this.Value.IsStable && this.Value.PropertyType == typeof(SolidColorBrush) && colorCanvas != null)
            //{
            //    for (int i = 0; i < Targets.Length; i++)
            //    {
            //        TargetDependencyProperties[i].RemoveValueChanged(Targets[i], Brush_Changed);
            //    }

            //    colorCanvas.SelectedColorChanged -= ColorCanvas_SelectedColorChanged;

            //    colorCanvas = null;
            //}
        }
    }
}
