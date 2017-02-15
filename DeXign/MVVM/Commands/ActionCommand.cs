using System;
using System.Windows.Input;

namespace DeXign.Commands
{
    public class ActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public event EventHandler<object> OnExecute;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            OnExecute?.Invoke(this, parameter);
        }
    }
}
