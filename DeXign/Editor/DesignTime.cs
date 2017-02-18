using System.Windows;

using WPFExtension;

namespace DeXign.Editor
{
    public static class DesignTime
    {
        public static readonly DependencyProperty DesignWidthProperty =
            DependencyHelper.RegisterAttached<double>();

        public static readonly DependencyProperty DesignHeightProperty =
            DependencyHelper.RegisterAttached<double>();

        public static readonly DependencyProperty DesignMinWidthProperty =
            DependencyHelper.RegisterAttached<double>();

        public static readonly DependencyProperty DesignMinHeightProperty =
            DependencyHelper.RegisterAttached<double>();

        public static readonly DependencyProperty DesignTagProperty =
            DependencyHelper.RegisterAttached<object>();

        #region [ Size ]
        public static void SetDesignWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(DesignWidthProperty, value);
        }

        public static void SetDesignHeight(this DependencyObject obj, double value)
        {
            obj.SetValue(DesignHeightProperty, value);
        }

        public static double GetDesignWidth(this DependencyObject obj)
        {
            return (double)obj.GetValue(DesignWidthProperty);
        }

        public static double GetDesignHeight(this DependencyObject obj)
        {
            return (double)obj.GetValue(DesignHeightProperty);
        }
        #endregion

        #region [ Min Size ]
        public static void SetDesignMinWidth(this DependencyObject obj, double value)
        {
            obj.SetValue(DesignMinWidthProperty, value);
        }

        public static void SetDesignMinHeight(this DependencyObject obj, double value)
        {
            obj.SetValue(DesignMinHeightProperty, value);
        }

        public static double GetDesignMinWidth(this DependencyObject obj)
        {
            return (double)obj.GetValue(DesignMinWidthProperty);
        }

        public static double GetDesignMinHeight(this DependencyObject obj)
        {
            return (double)obj.GetValue(DesignMinHeightProperty);
        }
        #endregion

        #region [ Tag ]
        public static void SetDesignTag(this DependencyObject obj, object value)
        {
            obj.SetValue(DesignTagProperty, value);
        }

        public static object GetDesignTag(this DependencyObject obj)
        {
            return obj.GetValue(DesignTagProperty);
        }
        #endregion
    }
}
