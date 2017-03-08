using System.Windows;
using System.Windows.Controls;

using WPFExtension;

namespace DeXign.Controls
{
    public class ContentCell : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty HeaderHorizontalAlignmentProperty =
            DependencyHelper.Register(new PropertyMetadata(HorizontalAlignment.Right));

        public static readonly DependencyProperty HeaderVerticalAlignmentProperty =
            DependencyHelper.Register(new PropertyMetadata(VerticalAlignment.Center));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public VerticalAlignment HeaderVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(HeaderVerticalAlignmentProperty); }
            set { SetValue(HeaderVerticalAlignmentProperty, value); }
        }

        public HorizontalAlignment HeaderHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderHorizontalAlignmentProperty); }
            set { SetValue(HeaderHorizontalAlignmentProperty, value); }
        }

        public ContentCell()
        {
            this.Focusable = false;
        }
    }
}
