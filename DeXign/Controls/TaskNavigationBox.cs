using DeXign.Task;
using System.Windows;
using System.Windows.Controls;
using WPFExtension;
using System;
using System.Windows.Data;
using DeXign.Extension;

namespace DeXign.Controls
{
    public class TaskNavigationBox : ContentControl
    {
        public static readonly DependencyProperty TaskManagerProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty CanUndoProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty CanRedoProperty =
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

        public bool CanUndo
        {
            get { return (bool)GetValue(CanUndoProperty); }
            set { SetValue(CanUndoProperty, value); }
        }

        public bool CanRedo
        {
            get { return (bool)GetValue(CanRedoProperty); }
            set { SetValue(CanRedoProperty, value); }
        }

        public TaskNavigationBox()
        {
            TaskManagerProperty.AddValueChanged(this, TaskManager_Changed);
        }

        private void TaskManager_Changed(object sender, EventArgs e)
        {
            if (TaskManager != null)
            {
                var expression = BindingOperations.GetBindingExpression(TaskManager, TaskManager.CanRedoProperty);

                if (expression == null)
                {
                    BindingEx.SetBinding(
                        TaskManager, TaskManager.CanRedoProperty,
                        this, TaskNavigationBox.CanRedoProperty,
                        BindingMode.OneWay);

                    BindingEx.SetBinding(
                        TaskManager, TaskManager.CanUndoProperty,
                        this, TaskNavigationBox.CanUndoProperty,
                        BindingMode.OneWay);

                    Console.WriteLine("Binded TaskManager to UI");
                }
            }
        }
    }
}
