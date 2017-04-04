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
            get { return this.GetValue<TaskManager>(TaskManagerProperty); }
            set
            {
                SetValue(TaskManagerProperty, value);
                this.DataContext = value;
            }
        }

        public bool CanUndo
        {
            get { return this.GetValue<bool>(CanUndoProperty); }
            set { SetValue(CanUndoProperty, value); }
        }

        public bool CanRedo
        {
            get { return this.GetValue<bool>(CanRedoProperty); }
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
                    BindingHelper.SetBinding(
                        TaskManager, TaskManager.CanRedoProperty,
                        this, TaskNavigationBox.CanRedoProperty,
                        BindingMode.OneWay);

                    BindingHelper.SetBinding(
                        TaskManager, TaskManager.CanUndoProperty,
                        this, TaskNavigationBox.CanUndoProperty,
                        BindingMode.OneWay);

                    Console.WriteLine("Binded TaskManager to UI");
                }
            }
        }
    }
}
