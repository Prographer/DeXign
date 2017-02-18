using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Core.Designer;
using DeXign.Editor.Controls;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Models;
using DeXign.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DeXign.Windows.Pages
{
    public partial class StoryboardPage : Page, IViewModel<StoryboardModel>
    {
        public StoryboardModel Model { get; set; }
        
        public StoryboardPage()
        {
            InitializeComponent();

            SetTheme(Platform.Android);

            Model = new StoryboardModel(this);
            this.DataContext = Model;

            Model.PlatformCommand.OnExecute += PlatformCommand_OnExecute;
            
            // test code
            storyboard.ElementChanged += Storyboard_ElementChanged;
            storyboard.Loaded += Storyboard_Loaded;
        }

        private void Storyboard_Loaded(object sender, RoutedEventArgs e)
        {
            storyboard.AddNewScreen();
        }

        private void Storyboard_ElementChanged(object sender, EventArgs e)
        {
            PresentXamlCode();
        }

        public void PresentXamlCode()
        {
            var screen = storyboard.Screens.FirstOrDefault();

            if (screen == null)
                return;

            var content = (PContentPage)screen.DataContext;
            LayoutExtension.SetPageName(content, "MainPage");

            // Generate
            var codeUnit = new CodeGeneratorUnit<PObject>()
            {
                NodeIterating = true,
                Items =
                {
                    content
                }
            };

            var assemblyInfo = new CodeGeneratorAssemblyInfo();
            var manifest = new CodeGeneratorManifest();

            var xGenerator = new XFormsGenerator(
                XFormsGenerateType.Xaml,
                codeUnit,
                manifest,
                assemblyInfo);

            string[] codes = xGenerator.Generate().ToArray();
            code.Text = codes[0];
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
