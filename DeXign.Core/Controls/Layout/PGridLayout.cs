using DeXign.Core.Collections;
using DeXign.Extension;
using System;
using System.Windows;

namespace DeXign.Core.Controls
{
    [XForms("Xamarin.Forms", "Grid")]
    public class PGridLayout : PLayout<PControl>
    {
        public static readonly DependencyProperty ColumnSpacingProperty = 
            DependencyHelper.Register();

        public static readonly DependencyProperty RowSpacingProperty =
            DependencyHelper.Register();

        public static readonly DependencyPropertyKey ColumnDefinitionsPropertyKey =
            DependencyHelper.RegisterReadonly(
                new FrameworkPropertyMetadata(
                    new PDefinitionCollection<PColumnDefinition>()));

        public static readonly DependencyPropertyKey RowDefinitionsPropertyKey =
            DependencyHelper.RegisterReadonly(
                new FrameworkPropertyMetadata(
                    new PDefinitionCollection<PRowDefinition>()));

        public static readonly DependencyProperty ColumnProperty =
            DependencyHelper.RegisterAttached<int>(
                    new FrameworkPropertyMetadata(
                        0, new PropertyChangedCallback(OnCellAttachedPropertyChanged)));

        public static readonly DependencyProperty RowProperty =
            DependencyHelper.RegisterAttached<int>(
                    new FrameworkPropertyMetadata(
                        0, new PropertyChangedCallback(OnCellAttachedPropertyChanged)));

        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyHelper.RegisterAttached<int>(
                    new FrameworkPropertyMetadata(
                        1, new PropertyChangedCallback(OnCellAttachedPropertyChanged)));

        public static readonly DependencyProperty RowSpanProperty =
            DependencyHelper.RegisterAttached<int>(
                    new FrameworkPropertyMetadata(
                        1, new PropertyChangedCallback(OnCellAttachedPropertyChanged)));

        private static void OnCellAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO: Update
        }

        [XForms("ColumnSpacing")]
        public double ColumnSpacing
        {
            get { return GetValue<double>(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        [XForms("RowSpacing")]
        public double RowSpacing
        {
            get { return GetValue<double>(RowSpacingProperty); }
            set { SetValue(RowSpacingProperty, value); }
        }

        [XForms("ColumnDefinitions")]
        public PDefinitionCollection<PColumnDefinition> ColumnDefinitions
        {
            get
            {
                return GetValue<PDefinitionCollection<PColumnDefinition>>(
                    ColumnDefinitionsPropertyKey.DependencyProperty);
            }
        }

        [XForms("RowDefinitions")]
        public PDefinitionCollection<PRowDefinition> RowDefinitions
        {
            get
            {
                return GetValue<PDefinitionCollection<PRowDefinition>>(
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
    }
}
