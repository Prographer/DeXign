using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeXign.Controls
{
    public class SubmitTextBox : TextBox
    {
        public event EventHandler Submit;

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var be = GetBindingExpression(TextProperty);

                be?.UpdateSource();

                Submit?.Invoke(this, EventArgs.Empty);
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnIsKeyboardFocusedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusedChanged(e);
            
            this.SelectAll();
        }
    }

}
