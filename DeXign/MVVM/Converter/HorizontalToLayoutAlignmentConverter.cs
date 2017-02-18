using System.Windows;

using DeXign.Core;

namespace DeXign.Converter
{
    public class HorizontalToLayoutAlignmentConverter : BaseValueConverter<HorizontalAlignment, LayoutAlignment>
    {
        public override LayoutAlignment Convert(HorizontalAlignment value, object parameter)
        {
            switch (value)
            {
                case HorizontalAlignment.Center:
                    return LayoutAlignment.Center;

                case HorizontalAlignment.Right:
                    return LayoutAlignment.End;

                case HorizontalAlignment.Left:
                    return LayoutAlignment.Start;

                default:
                    return LayoutAlignment.Fill;
            }
        }

        public override HorizontalAlignment ConvertBack(LayoutAlignment value, object parameter)
        {
            switch (value.Alignment)
            {
                case LayoutOptions.Center:
                    return HorizontalAlignment.Center;

                case LayoutOptions.End:
                    return HorizontalAlignment.Right;

                case LayoutOptions.Start:
                    return HorizontalAlignment.Left;

                default:
                    return HorizontalAlignment.Stretch;
            }
        }
    }
}
