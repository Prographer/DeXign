using System.Windows.Controls;
using System.Windows.Input;

namespace DeXign.Controls
{
    public class SubmitTextBox : TextBox
    {
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var be = GetBindingExpression(TextProperty);

                be?.UpdateSource();
            }

            base.OnPreviewKeyDown(e);
        }
    }

}
