using DeXign.Extension;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeXign.Controls
{
    [Setter(Type = typeof(double))]
    class DoubleSetter : ValueBoxSetter
    {
        public DoubleSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }
    }
}
