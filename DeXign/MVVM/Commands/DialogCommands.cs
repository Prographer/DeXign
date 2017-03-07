using System.Windows.Input;

namespace DeXign.Commands
{
    public static class DialogCommands
    {
        public static RoutedCommand CancelCommand { get; }
        public static RoutedCommand OkCommand { get; }

        public static RoutedCommand NextCommand { get; }
        public static RoutedCommand PreviousCommand { get; }

        static DialogCommands()
        {
            PreviousCommand = new RoutedCommand("Previous", typeof(DialogCommands));
            NextCommand = new RoutedCommand("Next", typeof(DialogCommands));
            CancelCommand = new RoutedCommand("Cancel", typeof(DialogCommands));
            OkCommand = new RoutedCommand("Ok", typeof(DialogCommands));
        }
    }
}
