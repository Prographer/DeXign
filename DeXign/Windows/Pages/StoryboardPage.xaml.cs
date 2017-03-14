using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;

using DeXign.Core;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Models;
using DeXign.Theme;
using DeXign.Editor;
using System.Linq;
using DeXign.Core.Logic;
using DeXign.Editor.Layer;
using DeXign.Editor.Logic;

namespace DeXign.Windows.Pages
{
    public partial class StoryboardPage : Page, IViewModel<StoryboardModel>
    {
        public StoryboardModel Model { get; set; }
        
        DispatcherTimer updateTimer;

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

            if (Model.Project.Screens.Count == 0)
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
            var theme = ThemeManager.GetTheme(platform);

            if (theme != null)
                this.Resources = theme;
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IRenderer screenRenderer = Model.Project.Screens[0].GetRenderer();
            FrameworkElement element = screenRenderer.Element;

            Rect bound = new Rect(
                    element.TranslatePoint(new Point(), storyboard),
                    element.RenderSize);

            bound.Inflate(100, 100);

            zoomPanel.ZoomFit(bound, true);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IRenderer r = null;
            object item = GroupSelector.GetSelectedItems().FirstOrDefault();

            if (item is StoryboardLayer l)
                r = (IRenderer)l;

            if (item is ComponentElement ce)
                r = ce.GetRenderer();

            if (r == null)
                return;

            var rb = RendererManager.ResolveBinder(r);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine($"Source: {r.GetType().Name}");
            sb.AppendLine();
            sb.AppendLine("Inputs:");

            foreach (BaseBinder input in rb.Inputs)
            {
                sb.AppendLine($"  {input.GetRenderer().GetType().Name}");
            }

            sb.AppendLine();
            sb.AppendLine("Outputs:");

            foreach (BaseBinder output in rb.Outputs)
            {
                sb.AppendLine($"  {output.GetRenderer().GetType().Name}");
            }

            MessageBox.Show(sb.ToString());
        }
    }
}