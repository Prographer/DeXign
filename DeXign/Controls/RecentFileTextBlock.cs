using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFExtension;

namespace DeXign.Controls
{
    class RecentFileTextBlock : Button
    {
        public static readonly DependencyProperty FileNameProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty TitleProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty SubTitleProperty =
            DependencyHelper.Register();

        public string FileName
        {
            get { return this.GetValue<string>(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public string Title
        {
            get { return this.GetValue<string>(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string SubTitle
        {
            get { return this.GetValue<string>(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }

        public RecentFileTextBlock()
        {
            FileNameProperty.AddValueChanged(this, FileNamed_Changed);
        }

        private void FileNamed_Changed(object sender, EventArgs e)
        {
            Title = Path.GetFileNameWithoutExtension(FileName);
            SubTitle = Path.GetDirectoryName(FileName);
        }
    }
}