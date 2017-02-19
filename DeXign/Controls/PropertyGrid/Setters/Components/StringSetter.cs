using System.Reflection;
using System.Windows;

namespace DeXign.Controls
{
    [Setter(Type = typeof(string))]
    class StringSetter : ValueBoxSetter
    {
        public StringSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }
    }
}
