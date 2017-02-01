using System.Windows.Input;

namespace DeXign
{
    public static class DXCommands
    {
        public static RoutedCommand NewProjectCommand { get; }
        public static RoutedCommand OpenProjectCommand { get; }

        static DXCommands()
        {
            NewProjectCommand = new RoutedCommand("NewProject", typeof(DXCommands));
            OpenProjectCommand = new RoutedCommand("OpenProject", typeof(DXCommands));
        }
    }
 }