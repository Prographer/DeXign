using DeXign.Core.Logic;

using WPFExtension;

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
                BindingHelper.SetBinding(
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