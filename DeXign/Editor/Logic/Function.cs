using DeXign.Core.Logic;
using DeXign.Extension;

namespace DeXign.Editor.Logic
{
    public class Function : ComponentElement
    {
        public new PFunction Model => (PFunction)base.Model;

        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            BindingEx.SetBinding(
                Model, PFunction.FunctionNameProperty,
                this, HeaderProperty);
        }
    }
}
