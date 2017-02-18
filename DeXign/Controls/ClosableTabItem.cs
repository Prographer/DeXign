using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeXign.Controls
{
    public class ClosableTabItem : TabItem
    {
        public static RoutedCommand CloseCommand { get; }

        static ClosableTabItem()
        {
            CloseCommand = new RoutedCommand("Close", typeof(ClosableTabItem));
        }

        public ClosableTabItem()
        {
            this.CommandBindings.Add(new CommandBinding(CloseCommand, Close));
        }

        private void Close(object sender, ExecutedRoutedEventArgs e)
        {
            var tab = this.Parent as TabControl;

            tab.Items.Remove(this);
        }
    }
}
