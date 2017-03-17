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

        public PParameterBinder(IBinderHost host) : base(host, BindOptions.Parameter)
        {
        }

        protected override void OnPropagateBind(IBinder output, IBinder input)
        {
            IBinder triggerReturnBinder = this.Items
                .FirstOrDefault(b => b.Host is PTrigger && b.BindOption == BindOptions.Return);

            if (triggerReturnBinder != null)
            {
                var triggerReturn = triggerReturnBinder as PReturnBinder;
                var trigger = triggerReturnBinder.Host as PTrigger;
                
                if (this.Host is PGetter getter && input.Equals(getter.TargetBinder))
                {
                    getter.TargetType = triggerReturn.ReturnType;
                }

                if (this.Host is PSetter setter && input.Equals(setter.TargetBinder))
                {
                    setter.TargetType = triggerReturn.ReturnType;
                }
            }
        }
    }
}