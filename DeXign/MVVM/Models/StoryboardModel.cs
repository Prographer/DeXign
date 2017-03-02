using DeXign.Commands;
using DeXign.Windows.Pages;
using DeXign.Task;
using System;

namespace DeXign.Models
{
    public class StoryboardModel : IModel
    {
        public StoryboardPage ViewModel { get; }

        public TaskManager TaskManager { get; }

        public ActionCommand PlatformCommand { get; set; }
        
        public StoryboardModel()
        {
            PlatformCommand = new ActionCommand();
            TaskManager = new TaskManager();

            TaskManager.Push(
                new TaskData(
                    this,
                    () =>
                    {
                        Console.WriteLine("Do");
                    },
                    () =>
                    {
                        Console.WriteLine("Redo");
                    }));
        }

        public StoryboardModel(StoryboardPage viewModel) : this()
        {
            this.ViewModel = viewModel;
        }
    }
}
