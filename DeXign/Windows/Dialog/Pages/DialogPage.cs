using System.Windows.Controls;

namespace DeXign.Windows.Pages
{
    public class DialogPage : Page, IDialogNavigator
    {
        public virtual bool CanNext()
        {
            return true;
        }

        public virtual bool CanOk()
        {
            return true;
        }

        public virtual bool CanPrevious()
        {
            return true;
        }
    }
}
