using System.IO;
using System.Windows;
using System.Windows.Input;

using DeXign.IO;
using DeXign.Models;
using DeXign.Database;
using DeXign.Extension;

namespace DeXign.Windows.Pages
{
    public partial class MainPage : DialogPage
    {
        public DXProject Project { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            InitializeCommands();
            InitializeRecents();
        }

        private void InitializeRecents()
        {
            recentList.Items.AddRange(RecentDB.GetFiles());
        }

        private void InitializeCommands()
        {
            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.OpenProjectCommand, OpenProject_Execute));
        }
        
        private void OpenProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is RecentItem item)
            {
                // 파일 체크
                if (!File.Exists(item.FileName))
                {
                    MessageBoxResult result =
                        MessageBox.Show(
                            $"'{item.FileName}' 파일을 열 수 없습니다. 이 파일에 대한 참조를 최근에 사용한 파일 목록에서 제거하시겠습니까?",
                            "DeXign",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        recentList.Items.Remove(item);
                        RecentDB.RemoveFile(item.FileName);
                    }

                    return;
                }

                // 프로젝트 열기
                this.Project = DXProject.Open(item.FileName);

                // 프로젝트 파일 열기 실패
                if (!this.Project.CanOpen)
                {
                    // 메박 커스텀하고 내용 바꿀..
                    MessageBox.Show("어디 나사하나 빠진 파일 같습니다.");
                    return;
                }
            }
            else
            {
                this.Project = DXProject.OpenDialog();
            }

            if (this.Project == null)
                return;

            this.CloseDialog(true);
        }
    }
}