using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

using DeXign.Controls;
using DeXign.Utilities;

using WPFExtension;

namespace DeXign.Behavior
{
    class NamespaceTextBoxBehavior : Behavior<DependencyObject>
    {
        private TextBox attachedTextBox;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject is TextCell)
            {
                TextCell.TextBoxProperty.AddValueChanged(AssociatedObject, TextBox_Attached);
            }
            else if (AssociatedObject is TextBox)
            {
                AttachTextBox(AssociatedObject as TextBox);
            }
        }

        protected override void OnDetaching()
        {
            Masking.SetMask(attachedTextBox, null);
            attachedTextBox = null;

            base.OnDetaching();
        }

        private void TextBox_Attached(object sender, EventArgs e)
        {
            AttachTextBox(((TextCell)AssociatedObject).TextBox);
        }

        private void AttachTextBox(TextBox textBox)
        {
            attachedTextBox = textBox;

            Masking.SetMask(attachedTextBox, @"^[a-zA-Z][a-zA-Z0-9\._-]*$");
        }
    }
}
