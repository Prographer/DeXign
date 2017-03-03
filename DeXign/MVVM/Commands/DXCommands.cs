using System.Windows.Input;

namespace DeXign
{
    public static class DXCommands
    {
        public static RoutedCommand NewProjectCommand { get; }

        public static RoutedCommand OpenProjectCommand { get; }

        public static RoutedCommand ESCCommand { get; }

        public static RoutedCommand PlatformCommand { get; }

        public static RoutedCommand DeleteCommand { get; }

        public static RoutedCommand DesignModeCommand { get; }

        public static RoutedCommand UndoCommand { get; }
        public static RoutedCommand RedoCommand { get; }

        static DXCommands()
        {
            NewProjectCommand = new RoutedCommand("NewProject", typeof(DXCommands));
            OpenProjectCommand = new RoutedCommand("OpenProject", typeof(DXCommands));
            ESCCommand = new RoutedCommand("ESCCommand", typeof(DXCommands));
            PlatformCommand = new RoutedCommand("PlatformCommand", typeof(DXCommands));
            DeleteCommand = new RoutedCommand("DeleteCommand", typeof(DXCommands));
            DesignModeCommand = new RoutedCommand("DesignModeCommand", typeof(DXCommands));
            UndoCommand = new RoutedCommand("UndoCommand", typeof(DXCommands));
            RedoCommand = new RoutedCommand("RedoCommand", typeof(DXCommands));
        }
    }
 }