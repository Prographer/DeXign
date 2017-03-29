using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using DeXign.Theme;
using DeXign.Models;
using DeXign.Extension;

namespace DeXign.Windows.Pages
{
    public partial class StoryboardPage : Page, IViewModel<StoryboardModel>
    {
        public StoryboardModel Model { get; set; }
        
        #region [ Constructor ]
        public StoryboardPage()
        {
            InitializeComponent();
            InitializeCommands();
            InitializeModel();

            SetTheme(Platform.Android);

            this.Loaded += StoryboardPage_Loaded;
            storyboard.Loaded += Storyboard_Loaded;

            // Task Manager Setting
            storyboard.TaskManager = Model.TaskManager;
        }

        private void StoryboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(storyboard);
        }

        private void InitializeModel()
        {
            Model = new StoryboardModel(this);
            this.DataContext = Model;

            Model.PlatformCommand.OnExecute += PlatformCommand_OnExecute;
        }

        private void InitializeCommands()
        {
            this.CommandBindings.Add(
                new CommandBinding(DXCommands.CloseCommand, Close_Execute));
        }
        #endregion

        #region [ Command ]
        private void Close_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            storyboard.Close();
        }
        #endregion
        
        private void Storyboard_Loaded(object sender, RoutedEventArgs e)
        {
            storyboard.Loaded -= Storyboard_Loaded;

            if (Model.Project.Screens.Count + Model.Project.Components.Count == 0)
            {
                storyboard.AddNewScreen();
            }
            else
            {
                storyboard.InitializeProject();
            }
        }

        #region [ Theme ]
        private void PlatformCommand_OnExecute(object sender, object e)
        {
            SetTheme(((string)e).ToEnum<Platform>().Value);
        }

        public void SetTheme(Platform platform)
        {
            this.Model.SelectedPlatform = platform;

            var theme = ThemeManager.GetTheme(platform);

            if (theme != null)
                this.Resources = theme;
        }
        #endregion
    }
}