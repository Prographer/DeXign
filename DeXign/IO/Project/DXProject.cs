using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using System.Collections.Generic;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Database;
using DeXign.Editor.Renderer;
using DeXign.Extension;

using Microsoft.Win32;

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
        Dictionary<Guid, RendererSurface> rendererInfos;
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
                LoadScreenRenderers();
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

        private void LoadScreenRenderers()
        {
            var rendererFiles = packageFiles
                .Where(pf => pf.Name.StartsWith($"{ScreenRendererPackageFile.Path}\\"));

            rendererInfos = new Dictionary<Guid, RendererSurface>();

            foreach (PackageFile file in rendererFiles)
            {
                var reader = new ObjectXmlReader(file.Stream);

                var container = reader.ReadObject() as RendererContainer;

                foreach (var r in container.Items)
                    rendererInfos[r.Guid] = r;
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

        #region [ Method ]
        public RendererSurface GetRendererSurface(Guid guid)
        {
            if (BoolEx.Nomalize(rendererInfos?.ContainsKey(guid)))
                return rendererInfos[guid];

            return null;
        }
        #endregion

        #region [ Static Method ]
        public static DXProject Create(string path, DXProjectManifest manifest)
        {
            RecentDB.AddFile(Path.GetFullPath(path));

            return new DXProject(path, manifest);
        }

        public static DXProject Open(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            var proj = new DXProject(path);

            if (proj.CanOpen)
                RecentDB.AddFile(Path.GetFullPath(path));

            return proj;
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