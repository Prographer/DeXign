using System;
using System.Windows.Input;

namespace DeXign.Commands
{
    public class RasableRoutedCommand : RoutedCommand, ICommand
    {
        // 외부로 이벤트 핸들링을 노출하지 않음

        private event EventHandler _canExecuteChanged;
        public new event EventHandler CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
                base.CanExecuteChanged += value;
            }
            remove
            {
                _canExecuteChanged -= value;
                base.CanExecuteChanged -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            _canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}