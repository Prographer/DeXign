using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using DeXign.Theme;
using DeXign.Models;
using DeXign.Extension;
using System;
using DeXign.Editor;
using System.Linq;
using DeXign.Controls;
using System.Windows.Threading;

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
            InitializeRuler();

            SetTheme(Platform.Android);

            this.Loaded += StoryboardPage_Loaded;
            storyboard.Loaded += Storyboard_Loaded;

            // Task Manager Setting
            storyboard.TaskManager = Model.TaskManager;
        }
        
        private void StoryboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(storyboard);

            this.Unloaded += StoryboardPage_Unloaded;
            GroupSelector.SelectedItemChanged += GroupSelector_SelectedItemChanged;
        }

        private void StoryboardPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= StoryboardPage_Unloaded;
            GroupSelector.SelectedItemChanged -= GroupSelector_SelectedItemChanged;
        }

        //private void ZoomPanel_Update(object sender, EventArgs e)
        //{
        //    Dispatcher.BeginInvoke(
        //        (Action)ruler.InvalidateVisual,
        //        DispatcherPriority.Render);
        //}


        private void InitializeRuler()
        {
            //zoomPanel.Zooming += ZoomPanel_Update;
            //zoomPanel.Panning += ZoomPanel_Update;
        }

        private void GroupSelector_SelectedItemChanged(object sender, EventArgs e)
        {
            //ruler.Target = GroupSelector.GetSelectedItems().FirstOrDefault();
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