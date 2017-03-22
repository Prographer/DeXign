using System;
using System.Windows;

namespace {RootNamespace}
{
    public class DXApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

             // Resources
            this.Resources = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/DeXign.UI;Component/DeXignUIStyle.xaml")
            };
        }
    }
}