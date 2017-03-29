﻿using System;
using System.Windows;
using System.Windows.Input;

using DeXign.IO;
using DeXign.Controls;
using DeXign.Database;
using DeXign.Models;
using System.Diagnostics;
using System.Windows.Controls;

namespace DeXign.Windows
{
    public partial class MainWindow : ChromeWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();
            InitializeRecents();
        }

        private void InitializeRecents()
        {
            foreach (RecentItem item in RecentDB.GetFiles())
            {
                recentList.Items.Add(item);
            }
        }

        private void InitializeCommands()
        {
            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.OpenProjectCommand, OpenProject_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.NewProjectCommand, NewProject_Execute));
        }

        private void NewProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.Hide();

            var projDialog = new ProjectDialog();

            if (!projDialog.ShowDialog())
            {
                this.Show();
                return;
            }
            
            var project = DXProject.Create(
                projDialog.FileName,
                new DXProjectManifest()
                {
                    ProjectName = projDialog.AppName,
                    PackageName = projDialog.PackageName
                });
            
            ShowEditorWindow(new EditorWindow(project));
        }

        private void OpenProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            DXProject project;

            if (e.Parameter is RecentItem item)
            {
                // 프로젝트 열기
                project = item.OpenDXProject();

                // 파일을 찾을 수 없음
                if (project == null)
                {
                    recentList.Items.Remove(item);
                    return;
                }

                // 프로젝트 열기 실패
                if (!project.CanOpen)
                {
                    return;
                }
            }
            else
            {
                project = DXProject.OpenDialog();
            }
            
            if (project == null)
                return;

            ShowEditorWindow(new EditorWindow(project));
        }

        private void ShowEditorWindow(EditorWindow window)
        {
            this.Hide();

            window.Closed += Window_Closed;
            window.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnDocument_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/mixnut/DeXign");
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://dexign.us/");
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://dexign.us/");
        }
    }
}
