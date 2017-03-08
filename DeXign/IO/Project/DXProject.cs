using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Editor.Renderer;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace DeXign.IO
{
    public class DXProject
    {
        #region [ Property ]
        public bool CanOpen { get; private set; } = true;

        public DXProjectManifest Manifest { get; internal set; }

        public string FileName { get; }

        public List<PContentPage> Screens { get; }
        #endregion

        #region [ Local Variable ]
        List<PackageFile> packageFiles;
        #endregion

        #region [ Constructor ]
        private DXProject()
        {
            packageFiles = new List<PackageFile>();

            this.Screens = new List<PContentPage>();
        }

        // Create
        internal DXProject(string path, DXProjectManifest manifest) : this()
        {
            this.FileName = path;
            this.Manifest = manifest;

            Save();
        }

        // Open
        internal DXProject(string path) : this()
        {
            this.FileName = path;

            try
            {
                LoadPackages();
                LoadManifest();
                LoadScreens();
            }
            catch (Exception ex)
            {
                CanOpen = false;
            }
        }
        #endregion

        #region [ Load ]
        private void LoadPackages()
        {
            packageFiles.Clear();

            using (var fs = File.OpenRead(FileName))
            {
                packageFiles.AddRange(
                    Package.Unpackaging(fs));
            }
        }

        private void LoadManifest()
        {
            // Manifest
            var manifestFile = packageFiles
                .FirstOrDefault(pf => pf.Name == ManifestPackageFile.FileName);

            if (manifestFile == null)
                return;

            var x = new XmlSerializer(typeof(DXProjectManifest));
            var manifest = x.Deserialize(manifestFile.Stream) as DXProjectManifest;

            this.Manifest = manifest;
        }

        private void LoadScreens()
        {
            Screens.Clear();

            var screenFiles = packageFiles
                .Where(pf => pf.Name.StartsWith("Screens\\"));

            foreach (PackageFile file in screenFiles)
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                var reader = new ObjectXmlReader(file.Stream);

                object model = reader.ReadObject();

                if (model is PContentPage)
                {
                    var screenModel = model as PContentPage;

                    // Name Setting
                    LayoutExtension.SetPageName(screenModel, name);

                    Screens.Add(screenModel);
                }
            }
        }
        #endregion

        #region [ Project Handling ]
        public void Close()
        {
            if (packageFiles != null)
                foreach (var pf in packageFiles)
                    pf.Dispose();

            packageFiles = null;
        }

        public void Save()
        {
            packageFiles.Clear();

            // Set Manifest
            packageFiles.Add(
                new ManifestPackageFile(this.Manifest));

            // Set Screens
            foreach (var screen in Screens)
                packageFiles.Add(
                    new ScreenPackageFile(screen));

            // Set Renderers
            foreach (var screen in Screens)
                packageFiles.Add(
                    new ScreenRendererPackageFile(screen.GetRenderer() as ScreenRenderer));

            // Save To File
            using (var fs = File.Open(FileName, FileMode.OpenOrCreate))
                Package.Packaging(fs, packageFiles);
        }
        #endregion

        #region [ Static Method ]
        public static DXProject Create(string path, DXProjectManifest manifest)
        {
            return new DXProject(path, manifest);
        }

        public static DXProject Open(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            return new DXProject(path);
        }

        public static DXProject OpenDialog()
        {
            var fileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "DeXign 프로젝트 파일(*.dx)|*.dx"
            };

            bool? result = fileDialog.ShowDialog();

            if (result != null && result.Value)
            {
                var project = DXProject.Open(fileDialog.FileName);

                if (!project.CanOpen)
                {
                    // 메박 커스텀하고 내용 바꿀..
                    MessageBox.Show("어디 나사하나 빠진 파일 같습니다.");
                    return null;
                }

                return project;
            }

            return null;
        }
        #endregion
    }
}