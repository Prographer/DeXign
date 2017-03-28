using System;
using System.Windows;
using System.Windows.Controls;
using WPFExtension;

namespace DeXign.Task
{
    public static class TaskManagerHelper
    {
        private static readonly DependencyProperty TaskManagerProperty =
            DependencyHelper.RegisterAttached<TaskManager>();

        private static readonly DependencyProperty LockUpdateProperty =
            DependencyHelper.RegisterAttached<bool>();

        private static readonly DependencyProperty TagProperty =
            DependencyHelper.RegisterAttached<object>();

        private static TaskManager GetTaskManager(this DependencyObject obj)
        {
            return (TaskManager)obj.GetValue(TaskManagerProperty);
        }

        private static void SetTaskManager(this DependencyObject obj, TaskManager manager)
        {
            obj.SetValue(TaskManagerProperty, manager);
        }

        private static void Lock(this DependencyObject obj)
        {
            obj.SetValue(LockUpdateProperty, true);
        }

        private static void Unlock(this DependencyObject obj)
        {
            obj.SetValue(LockUpdateProperty, false);
        }

        private static bool IsLocked(this DependencyObject obj)
        {
            return (bool)obj.GetValue(LockUpdateProperty);
        }

        private static object GetTag(this DependencyObject obj)
        {
            return obj.GetValue(TagProperty);
        }

        private static void SetTag(this DependencyObject obj, object tag)
        {
            obj.SetValue(TagProperty, tag);
        }

        #region [ CheckBox ]
        public static void HookCheckBox(this TaskManager manager, CheckBox checkBox)
        {
            checkBox.SetTaskManager(manager);

            CheckBox.IsCheckedProperty.AddValueChanged(checkBox, CheckBox_IsCheckedChanged);
        }

        public static void UnHookCheckBox(this TaskManager manager, CheckBox checkBox)
        {
            checkBox.SetTaskManager(null);

            CheckBox.IsCheckedProperty.RemoveValueChanged(checkBox, CheckBox_IsCheckedChanged);
        }

        private static void CheckBox_IsCheckedChanged(object sender, EventArgs e)
        {
            var chkBox = sender as CheckBox;
            TaskManager taskManager = chkBox.GetTaskManager();

            if (chkBox.IsLocked())
                return;

            bool state = chkBox.IsChecked.Value;

            taskManager.Push(
                new TaskData(chkBox,
                () =>
                {
                    chkBox.Lock();
                    chkBox.IsChecked = state;
                    chkBox.Unlock();
                },
                () =>
                {
                    chkBox.Lock();
                    chkBox.IsChecked = !state;
                    chkBox.Unlock();
                }));
        }
        #endregion

        #region [ ComboBox ]
        public static void HookComboBox(this TaskManager manager, ComboBox comboBox)
        {
            comboBox.SetTaskManager(manager);
            comboBox.SetTag(comboBox.SelectedItem);

            ComboBox.SelectedItemProperty.AddValueChanged(comboBox, ComboBox_SelectedItemChanged);
        }
        
        public static void UnHookComboBox(this TaskManager manager, ComboBox comboBox)
        {
            comboBox.SetTaskManager(null);
            comboBox.SetTag(null);

            ComboBox.SelectedItemProperty.RemoveValueChanged(comboBox, ComboBox_SelectedItemChanged);
        }

        private static void ComboBox_SelectedItemChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            TaskManager taskManager = comboBox.GetTaskManager();

            if (comboBox.IsLocked())
                return;

            object undoSelectedItem = comboBox.GetTag();
            object doSelectedItem = comboBox.SelectedItem;

            comboBox.SetTag(doSelectedItem);

            taskManager.Push(
                new TaskData(comboBox,
                () =>
                {
                    comboBox.Lock();
                    comboBox.SelectedItem = doSelectedItem;
                    comboBox.Unlock();
                },
                () =>
                {
                    comboBox.Lock();
                    comboBox.SelectedItem = undoSelectedItem;
                    comboBox.Unlock();
                }));
        }
        #endregion
    }
}
