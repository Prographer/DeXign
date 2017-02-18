using System.Windows;

using DeXign.Core;

namespace DeXign.Converter
{
    public class VerticalToLayoutAlignmentConverter : BaseValueConverter<VerticalAlignment, LayoutAlignment>
    {
        public override LayoutAlignment Convert(VerticalAlignment value, object parameter)
        {
            switch (value)
            {
                case VerticalAlignment.Center:
                    return LayoutAlignment.Center;

                case VerticalAlignment.Bottom:
                    return LayoutAlignment.End;

                case VerticalAlignment.Top:
                    return LayoutAlignment.Start;

                default:
                    return LayoutAlignment.Fill;
            }
        }

        public override VerticalAlignment ConvertBack(LayoutAlignment value, object parameter)
        {
            switch (value.Alignment)
            {
                case LayoutOptions.Center:
                    return VerticalAlignment.Center;

                case LayoutOptions.End:
                    return VerticalAlignment.Bottom;

                case LayoutOptions.Start:
                    return VerticalAlignment.Top;

                default:
                    return VerticalAlignment.Stretch;
            }
        }
    }
}
