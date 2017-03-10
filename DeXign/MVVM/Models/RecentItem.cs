using DeXign.Database;
using DeXign.IO;
using System;
using System.IO;
using System.Windows;

namespace DeXign.Models
{
    public class RecentItem
    {
        public DateTime LastedTime { get; set; }

        public int Id { get; set; }

        public string FileName { get; set; }

        public RecentItem()
        {
        }

        public RecentItem(string fileName)
        {
            this.FileName = fileName;
            this.LastedTime = DateTime.Now;
        }

        public override string ToString()
        {
            return FileName;
        }

        public DXProject OpenDXProject()
        {
            if (!File.Exists(this.FileName))
            {
                MessageBoxResult result =
                    MessageBox.Show(
                        $"'{this.FileName}' 파일을 열 수 없습니다. 이 파일에 대한 참조를 최근에 사용한 파일 목록에서 제거하시겠습니까?",
                        "DeXign",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    RecentDB.RemoveFile(this.FileName);

                    return null;
                }
            }

            return DXProject.Open(this.FileName);
        }
    }
}
