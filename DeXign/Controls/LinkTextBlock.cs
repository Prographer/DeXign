using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WPFExtension;

namespace DeXign.Controls
{
    class LinkTextBlock : TextBlock, ICommandSource
    {
        public event EventHandler Click;

        public static readonly DependencyProperty CommandProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty LinkProperty =
            DependencyHelper.Register();

        public Uri Link
        {
            get { return (Uri)GetValue(LinkProperty); }
            set { SetValue(LinkProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter { get; set; }

        public IInputElement CommandTarget { get; set; }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (Command != null)
            {
                var command = (RoutedCommand)Command;
                
                if (command != null)
                    command.Execute(CommandParameter, CommandTarget);
                else
                    Command.Execute(CommandParameter);
            }

            if (Link != null)
                Process.Start(Link.ToString());

            Click?.Invoke(this, EventArgs.Empty);
        }
    }
}
