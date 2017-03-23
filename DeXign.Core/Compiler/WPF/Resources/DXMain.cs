using System;
using System.Windows;
using System.Windows.Controls;
using DeXign.UI;

namespace {RootNamespace}
{
    class Program
    {
        static DXAppWindow MainWindow { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            var app = new DXApp();
            var dw = new DXAppWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Title = "DeXign Compiled WPF Window"
            };
            try { 
            {PageInitialize}
            }catch(Exception ex) { MessageBox.Show(ex.ToString()); }

            MainWindow = dw;

            dw.Show();
            app.Run(dw);
        }
    }
}