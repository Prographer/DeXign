using Phlet.Extension;
using System.Windows;

namespace Phlet.Core.Controls
{
    public class PControl : DependencyObject
    {
        public static readonly DependencyProperty NameProperty =
            DependencyHelper.Register();

        [XForms("Name")]
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public PControl()
        {
        }

        public T GetValue<T>(DependencyProperty dp)
        {
            return (T)GetValue(dp);
        }

        public new void SetValue(DependencyProperty dp, object value)
        {
            base.SetValue(dp, value);
        }
    }
}