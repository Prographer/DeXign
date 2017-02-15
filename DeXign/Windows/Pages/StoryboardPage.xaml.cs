using DeXign.Extension;
using DeXign.Models;
using DeXign.Themes;
using System;
using System.Collections.Generic;
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
