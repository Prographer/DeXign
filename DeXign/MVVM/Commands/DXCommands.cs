using System.Windows.Input;

namespace DeXign
{
    public static class DXCommands
    {
        // New / Open / Save
        public static RoutedCommand NewProjectCommand { get; }
        public static RoutedCommand OpenProjectCommand { get; }
        public static RoutedCommand SaveProjectCommand { get; }

        // Close
        public static RoutedCommand CloseCommand { get; }

        public static RoutedCommand ESCCommand { get; }

        public static RoutedCommand PlatformCommand { get; }

        public static RoutedCommand DeleteCommand { get; }

        public static RoutedCommand DesignModeCommand { get; }

        // Undo / Redo
        public static RoutedCommand UndoCommand { get; }
        public static RoutedCommand RedoCommand { get; }

        static DXCommands()
        {
            NewProjectCommand = new RoutedCommand("NewProject", typeof(DXCommands));
            OpenProjectCommand = new RoutedCommand("OpenProject", typeof(DXCommands));
            SaveProjectCommand = new RoutedCommand("SaveProject", typeof(DXCommands));

            ESCCommand = new RoutedCommand("ESC", typeof(DXCommands));
            PlatformCommand = new RoutedCommand("Platform", typeof(DXCommands));
            DeleteCommand = new RoutedCommand("Delete", typeof(DXCommands));
            DesignModeCommand = new RoutedCommand("DesignMode", typeof(DXCommands));

            UndoCommand = new RoutedCommand("Undo", typeof(DXCommands));
            RedoCommand = new RoutedCommand("Redo", typeof(DXCommands));

            CloseCommand = new RoutedCommand("Close", typeof(DXCommands));
        }
    }
 }