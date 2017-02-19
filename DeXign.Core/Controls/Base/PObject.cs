using System.Windows;

using WPFExtension;

namespace DeXign.Core.Controls
{
    public class PObject : DependencyObject
    {
        public static readonly DependencyProperty NameProperty =
            DependencyHelper.Register();

        // for resources
        public string Id { get; set; }

        [DesignElement(DisplayName = "이름", Visible = false)]
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public PObject()
        {
        }

        public T GetValue<T>(DependencyProperty dp)
        {
            return (T)GetValue(dp);
        }
    }
}