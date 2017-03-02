using DeXign.Task;
using System.Windows;
using System.Windows.Controls;
using WPFExtension;

namespace DeXign.Controls
{
    public class ActionControlBox : ContentControl
    {
        public static readonly DependencyProperty TaskManagerProperty =
            DependencyHelper.Register();

        public TaskManager TaskManager
        {
            get { return (TaskManager)GetValue(TaskManagerProperty); }
            set
            {
                SetValue(TaskManagerProperty, value);
                this.DataContext = value;
            }
        }

        public ActionControlBox()
        {
        }
    }
}
