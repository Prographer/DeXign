using System;
using System.Windows;
using System.Windows.Markup;

using DeXign.Core.Collections;
using DeXign.Extension;

using WPFExtension;
using DeXign.SDK;

namespace DeXign.Core.Controls
{
    [ContentProperty("Children")]
    [DesignElement(Category = Constants.Designer.Layout, DisplayName = "그리드")]
    [XForms("Xamarin.Forms", "Grid")]
    [WPF("System.Windows.Controls", "Grid")]
    
    // WPF에서 제공하는 기본 Grid는 Padding 및 Spacing 속성이 없기때문에
    // 추후 그리드를 커스텀(구현)한 경우 속성 제외를 해제해야함
    [DXIgnore("Padding")]
    public class PGridLayout : PLayout<PControl>
    {
        public static readonly DependencyProperty ColumnSpacingProperty = 
            DependencyHelper.Register();

        public static readonly DependencyProperty RowSpacingProperty =
            DependencyHelper.Register();

        private static readonly DependencyPropertyKey ColumnDefinitionsPropertyKey =
            DependencyHelper.RegisterReadOnly();

        private static readonly DependencyPropertyKey RowDefinitionsPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static readonly DependencyProperty ColumnProperty =
            DependencyHelper.RegisterAttached<int>(
                    new FrameworkPropertyMetadata(
                        0, OnCellAttachedPropertyChanged));

        public static readonly DependencyProperty RowProperty =
            DependencyHelper.RegisterAttached<int>(
                    new FrameworkPropertyMetadata(
                        0, OnCellAttachedPropertyChanged));

        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyHelper.RegisterAttached<int>(
                    new FrameworkPropertyMetadata(
                        1, OnCellAttachedPropertyChanged));

        public static readonly DependencyProperty RowSpanProperty =
            DependencyHelper.RegisterAttached<int>(
                    new FrameworkPropertyMetadata(
                        1, OnCellAttachedPropertyChanged));

        private static void OnCellAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO: Update
        }

        [DesignElement(Category = Constants.Property.Blank, DisplayName = "가로축 공백")]
        [XForms("ColumnSpacing")]
        public double ColumnSpacing
        {
            get { return this.GetValue<double>(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        [DesignElement(Category = Constants.Property.Blank, DisplayName = "세로축 공백")]
        [XForms("RowSpacing")]
        public double RowSpacing
        {
            get { return this.GetValue<double>(RowSpacingProperty); }
            set { SetValue(RowSpacingProperty, value); }
        }

        [XForms("ColumnDefinitions")]
        public PDefinitionCollection<PColumnDefinition> ColumnDefinitions
        {
            get
            {
                return this.GetValue<PDefinitionCollection<PColumnDefinition>>(
                    ColumnDefinitionsPropertyKey.DependencyProperty);
            }
        }

        [XForms("RowDefinitions")]
        public PDefinitionCollection<PRowDefinition> RowDefinitions
        {
            get
            {
                return this.GetValue<PDefinitionCollection<PRowDefinition>>(
                    RowDefinitionsPropertyKey.DependencyProperty);
            }
        }

        public static void SetColumn(PObject control, int value)
        {
            if (control == null)
                throw new ArgumentNullException("element");

            control.SetValue(ColumnProperty, value);
        }

        public static void SetRow(PObject control, int value)
        {
            if (control == null)
                throw new ArgumentNullException("element");

            control.SetValue(RowProperty, value);
        }

        public static void SetColumnSpan(PObject control, int value)
        {
            if (control == null)
                throw new ArgumentNullException("element");

            control.SetValue(ColumnSpanProperty, value);
        }

        public static void SetRowSpan(PObject control, int value)
        {
            if (control == null)
                throw new ArgumentNullException("element");

            control.SetValue(RowSpanProperty, value);
        }

        public static int GetColumn(PObject control, int value)
        {
            if (control == null)
                throw new ArgumentNullException("element");

            return (int)control.GetValue(ColumnProperty);
        }

        public static int GetRow(PObject control, int value)
        {
            if (control == null)
                throw new ArgumentNullException("element");

            return (int)control.GetValue(RowProperty);
        }

        public static int GetColumnSpan(PObject control, int value)
        {
            if (control == null)
                throw new ArgumentNullException("element");

            return (int)control.GetValue(ColumnSpanProperty);
        }

        public static int GetRowSpan(PObject control, int value)
        {
            if (control == null)
                throw new ArgumentNullException("element");

            return (int)control.GetValue(RowSpanProperty);
        }

        public PGridLayout()
        {
            SetValue(
                ColumnDefinitionsPropertyKey, 
                new PDefinitionCollection<PColumnDefinition>());

            SetValue(
                RowDefinitionsPropertyKey,
                new PDefinitionCollection<PRowDefinition>());
        }
    }
}
