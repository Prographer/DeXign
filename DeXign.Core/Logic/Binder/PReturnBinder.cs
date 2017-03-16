using System;
using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic
{
    public class PReturnBinder : PNamedBinder
    {
        private static readonly DependencyPropertyKey ReturnTypePropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static readonly DependencyProperty ReturnTypeProperty =
            ReturnTypePropertyKey.DependencyProperty;

        public Type ReturnType
        {
            get { return GetValue<Type>(ReturnTypeProperty); }
        }

        public PReturnBinder(IBinderHost host, Type returnType) : base(host, BindOptions.Return)
        {
            SetValue(ReturnTypePropertyKey, returnType);
        }
    }
}