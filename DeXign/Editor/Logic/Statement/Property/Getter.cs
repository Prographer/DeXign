using System;
using System.Windows;
using System.Reflection;
using System.Collections.ObjectModel;

using DeXign.Core;
using DeXign.Extension;
using DeXign.Core.Designer;

using WPFExtension;
using DeXign.Core.Logic;
using System.Linq;
using System.Windows.Controls;

namespace DeXign.Editor.Logic
{    
    public class Getter : BaseProperty
    {
        protected PReturnBinder ReturnBinder { get; private set; }

        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            this.ReturnBinder = this.GetBinderModel<PReturnBinder>(BindOptions.Return, 0);

            if (this.ReturnBinder != null)
            {
                BindingEx.SetBinding(
                    this, BaseProperty.TargetTypeProperty,
                    this.ReturnBinder, PReturnBinder.ReturnTypeProperty);
            }
        }

        protected override void OnTargetTypeChanged()
        {
            base.OnTargetTypeChanged();

            var t = ReturnBinder.ReturnType;
        }
    }
}