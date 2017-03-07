using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;

using DeXign.Models;
using DeXign.Controls;
using DeXign.Commands;

using WinSystemCommands = System.Windows.SystemCommands;
using DeXign.Windows.Pages;

namespace DeXign.Windows
{
    public partial class DialogWindow : ChromeWindow, IViewModel<DialogWindowModel>
    {
        #region [ Property ]
        public DialogPage[] Pages { get; }

        public DialogWindowModel Model { get; set; }

        internal int PageInex { get; private set; } = -1;
        #endregion

        #region [ Local Variable ]
        private IDialogNavigator navigator;
        #endregion

        #region [ Constructor ]
        public DialogWindow(IEnumerable<DialogPage> pages)
        {
            InitializeComponent();
            InitializeCommands();

            Model = new DialogWindowModel();
            this.DataContext = Model;

            this.Pages = pages.ToArray();

            SetPage(0);
        }

        private void InitializeCommands()
        {
            this.CommandBindings.Add(
                new CommandBinding(
                    WinSystemCommands.CloseWindowCommand, WindowClose_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DialogCommands.PreviousCommand, Previous_Execute, Previous_CanExecute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DialogCommands.NextCommand, Next_Execute, Next_CanExecute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DialogCommands.OkCommand, OK_Execute, Ok_CanExecute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DialogCommands.CancelCommand, Cancel_Execute));
        }
        #endregion

        #region [ Commands ]
        private void Ok_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OnOkCanExecute();
        }

        private void Previous_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OnPreviousCanExecute();
        }

        private void Next_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OnNextCanExecute();
        }

        private void OK_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close(true);
        }

        private void Cancel_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close(false);
        }

        private void Previous_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            PreviousPage();
        }

        private void Next_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            NextPage();
        }

        private void WindowClose_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Close(false);
        }

        protected virtual bool OnPreviousCanExecute()
        {
            return (this.PageInex > 0) && 
                (navigator != null ? navigator.CanPrevious() : true);
        }

        protected virtual bool OnNextCanExecute()
        {
            return (this.PageInex < this.Pages.Length - 1) && 
                (navigator != null ? navigator.CanNext() : true);
        }

        protected virtual bool OnOkCanExecute()
        {
            return (navigator != null ? navigator.CanOk() : true);
        }
        #endregion

        #region [ Dialog Handling ]
        private void Close(bool dialogResult)
        {
            DialogResult = dialogResult;
            this.Close();
        }

        public void SetPage(int index)
        {
            index = Math.Max(index, 0);
            index = Math.Min(index, Pages.Length - 1);

            this.PageInex = index;

            Model.Page = Pages[index];
            navigator = Pages[index];
        }

        public void NextPage()
        {
            SetPage(this.PageInex + 1);
        }

        public void PreviousPage()
        {
            SetPage(this.PageInex - 1);
        }

        public new bool ShowDialog()
        {
            var result = base.ShowDialog();

            return result != null && result.Value;
        }
        #endregion
    }
}
