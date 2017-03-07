namespace DeXign.Windows
{
    interface IDialogNavigator
    {
        bool CanNext();
        bool CanPrevious();
        bool CanOk();
    }
}
