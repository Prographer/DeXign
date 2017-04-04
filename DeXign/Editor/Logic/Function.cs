using DeXign.Core.Logic;

using WPFExtension;

namespace DeXign.Editor.Logic
{
    public class Function : ComponentElement
    {
        public new PFunction Model => (PFunction)base.Model;

        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            BindingHelper.SetBinding(
                Model, PFunction.FunctionNameProperty,
                this, HeaderProperty);
        }
    }
}
