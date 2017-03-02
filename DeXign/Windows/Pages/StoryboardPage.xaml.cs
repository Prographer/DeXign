using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Models;
using DeXign.Theme;

namespace DeXign.Windows.Pages
{
    public partial class StoryboardPage : Page, IViewModel<StoryboardModel>
    {
        public StoryboardModel Model { get; set; }

        DispatcherTimer updateTimer;

        public StoryboardPage()
        {
            InitializeComponent();

            SetTheme(Platform.Android);

            Model = new StoryboardModel(this);
            this.DataContext = Model;

            Model.PlatformCommand.OnExecute += PlatformCommand_OnExecute;

            // Task Manager Setting
            storyboard.TaskManager = Model.TaskManager;

            // test code
            storyboard.Loaded += Storyboard_Loaded;

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(20);
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            var screenRenderer = storyboard.Screens[0].GetRenderer() as ScreenRenderer;

            LayoutExtension.SetPageName(screenRenderer.Model as PPage, "MainPage");
            
            // Generate
            var codeUnit = new CodeGeneratorUnit<PObject>()
            {
                NodeIterating = true,
                Items =
                {
                    screenRenderer.Model
                }
            };

            var assemblyInfo = new CodeGeneratorAssemblyInfo();
            var manifest = new CodeGeneratorManifest();

            var xGenerator = new XFormsGenerator(
                XFormsGenerateType.Xaml,
                codeUnit,
                manifest,
                assemblyInfo);

            foreach (string code in xGenerator.Generate())
            {
                codeBox.Text = code;
            }
        }

        private void Storyboard_Loaded(object sender, RoutedEventArgs e)
        {
            storyboard.Loaded -= Storyboard_Loaded;
            storyboard.AddNewScreen();
        }
        
        private void PlatformCommand_OnExecute(object sender, object e)
        {
            SetTheme(((string)e).ToEnum<Platform>().Value);
        }

        public void SetTheme(Platform platform)
        {
            var theme = ThemeManager.GetTheme(platform);

            if (theme != null)
                this.Resources = theme;
        }
    }
}
