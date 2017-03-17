using DeXign.Core.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeXign.Editor.Logic
{
    public class Setter : BaseProperty
    {
        protected PParameterBinder ValueBinder { get; private set; }

        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            this.ValueBinder= this.GetBinderModel<PParameterBinder>(BindOptions.Parameter, 1);
        }
    }
}
