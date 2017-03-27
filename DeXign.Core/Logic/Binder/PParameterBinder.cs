using DeXign.Extension;
using System;
using System.Linq;
using System.Windows;

using WPFExtension;

namespace DeXign.Core.Logic
{
    public class PParameterBinder : PNamedBinder
    {
        public static readonly DependencyProperty ParameterTypeProperty =
            DependencyHelper.Register();
        
        public Type ParameterType
        {
            get { return GetValue<Type>(ParameterTypeProperty); }
            set { SetValue(ParameterTypeProperty, value); }
        }

        public PParameterBinder()
        {
        }

        public PParameterBinder(IBinderHost host) : base(host, BindOptions.Parameter)
        {
        }

        public override void Bind(IBinder targetBinder)
        {
            base.Bind(targetBinder);

            if (targetBinder is PReturnBinder returnBinder)
            {
                BindingEx.SetBinding(
                    returnBinder, PReturnBinder.ReturnTypeProperty,
                    this, PParameterBinder.ParameterTypeProperty);
            }
        }

        protected override void OnPropagateBind(IBinder output, IBinder input)
        {
            IBinder triggerReturnBinder = this.Items
                .FirstOrDefault(b => b.Host is PTrigger && b.BindOption == BindOptions.Return);

            IBinder selectorReturnBinder = this.Items
                .FirstOrDefault(b => b.Host is PSelector && b.BindOption == BindOptions.Return);

            if (this.Host is PTargetable targetable && input.Equals(targetable.TargetBinder))
            {
                if (triggerReturnBinder != null)
                {
                    var triggerReturn = triggerReturnBinder as PReturnBinder;
                    var trigger = triggerReturnBinder.Host as PTrigger;
                    
                    targetable.TargetType = triggerReturn.ReturnType;
                }

                if (selectorReturnBinder != null)
                {
                    var selectorReturn = selectorReturnBinder as PReturnBinder;
                    var selector = selectorReturnBinder.Host as PSelector;

                    targetable.TargetType = selectorReturn.ReturnType;
                }
            }
        }
    }
}