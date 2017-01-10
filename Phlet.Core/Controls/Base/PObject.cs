using System.Windows;

namespace Phlet.Core.Controls
{
    public class PObject : DependencyObject
    {
        public PObject()
        {
        }

        public T GetValue<T>(DependencyProperty dp)
        {
            return (T)GetValue(dp);
        }
    }
}
