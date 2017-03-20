using System;
using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic
{
    public class PReturnBinder : PNamedBinder
    {
        public static readonly DependencyProperty ReturnTypeProperty =
            DependencyHelper.Register();

        public Type ReturnType
        {
            get { return GetValue<Type>(ReturnTypeProperty); }
            set { SetValue(ReturnTypeProperty, value); }
        }

        public PReturnBinder()
        {
        }

        public PReturnBinder(IBinderHost host, Type returnType) : base(host, BindOptions.Return)
        {
            this.ReturnType = returnType;
        }
    }
}