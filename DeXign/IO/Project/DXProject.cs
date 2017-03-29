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
using DeXign.Core.Logic;

using Microsoft.Win32;
using System.Reflection;
using System.Text;

namespace DeXign.IO
{
    public class DXProject
    {
        #region [ Property ]
        public bool CanOpen { get; private set; } = true;

        public DXProjectManifest Manifest { get; internal set; }

        public string FileName { get; }

        public List<PContentPage> Screens { get; }
        public List<PComponent> Components { get; }
        #endregion

        #region [ Local Variable ]
        List<PackageFile> packageFiles;
        Dictionary<Guid, RendererSurface> rendererInfos;
        Dictionary<Guid, PBinder> binderInfos;
        List<BindExpression> bindExpressions;
        #endregion

        #region [ Constructor ]
        private DXProject()
        {
            packageFiles = new List<PackageFile>();

            this.Screens = new List<PContentPage>();
            this.Components = new List<PComponent>();
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
            this.CanOpen = false;

            rendererInfos = new Dictionary<Guid, RendererSurface>();
            binderInfos = new Dictionary<Guid, PBinder>();
            
            // 패키지 파일 불러옴
            if (!LoadPackages())
            {
                MessageBox.Show("파일을 불러올 수 없습니다.", "DeXign", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // 매니페스트 불러옴
            if (!LoadManifest())
            {
                MessageBox.Show("프로젝트 구성파일을 찾을 수 없습니다.", "DeXign", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
       
            // Screen, Renderer, Component 구성 요소 불러옴
            if (!LoadScreens() ||
                !LoadScreenRenderers() ||
                !LoadComponents())
            {
                if (this.Manifest != null)
                {
                    string[] manifestReferences = this.Manifest.ReferencedModules.Items.ToArray();
                    string[] loadedReferences = SDKManager.GetReferencedModules().ToArray();

                    string[] exceptedModules = manifestReferences.Except(loadedReferences).ToArray();

                    if (exceptedModules.Length > 0)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("이 프로젝트를 열기위한 모듈을 찾을 수 없습니다.");
                        sb.AppendLine();

                        foreach (string module in exceptedModules)
                        {
                            sb.AppendLine(module.Split(',')[0]);
                        }

                        MessageBox.Show(sb.ToString(), "DeXign", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("알 수 없는 오류", "DeXign", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                return;
            }

            this.CanOpen = true;
        }
        #endregion

        #region [ Load ]
        private bool LoadPackages()
        {
            try
            {
                packageFiles.Clear();

                using (var fs = File.OpenRead(FileName))
                {
                    packageFiles.AddRange(
                        Package.Unpackaging(fs));
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        private bool LoadManifest()
        {
            try
            {
                // Manifest
                var manifestFile = packageFiles
                    .FirstOrDefault(pf => pf.Name == ManifestPackageFile.FileName);

                if (manifestFile == null)
                    return false;

                var x = new XmlSerializer(typeof(DXProjectManifest));
                var manifest = x.Deserialize(manifestFile.Stream) as DXProjectManifest;

                this.Manifest = manifest;

                return true;
            }
            catch
            {
            }

            return false;
        }

        private bool LoadScreens()
        {
            try
            {
                Screens.Clear();
                Components.Clear();

                var screenFiles = packageFiles
                    .Where(pf => pf.Name.IsMatch($"^{ScreenPackageFile.Path}/.+"));

                foreach (PackageFile file in screenFiles)
                {
                    string name = Path.GetFileNameWithoutExtension(file.Name);
                    var reader = new ObjectXmlReader(file.Stream);

                    object model = reader.ReadObject();

                    if (model is PContentPage page)
                    {
                        // Name Setting
                        LayoutExtension.SetPageName(page, name);

                        Screens.Add(page);

                        CachingBinder(page.Binder);

                        foreach (var node in ObjectContentTreeHelper.FindContentChildrens<PVisual, PVisual>(page))
                        {
                            CachingBinder(node.Child.Binder);
                        }
                    }
                }

                return true;
            }
            catch
            {
            }

            return false;
        }
        
        private bool LoadScreenRenderers()
        {
            try
            {
                var rendererFiles = packageFiles
                .Where(pf => pf.Name.IsMatch($"^{ScreenRendererPackageFile.Path}/.+"));

                foreach (PackageFile file in rendererFiles)
                {
                    var reader = new ObjectXmlReader(file.Stream);

                    var container = reader.ReadObject() as ObjectContainer<RendererSurface>;

                    foreach (var r in container.Items)
                        rendererInfos[r.Guid] = r;
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        private bool LoadComponents()
        {
            try
            {
                var componentFile = packageFiles
                .FirstOrDefault(pf => pf.Name == ComponentPackageFile.FileName);

                var expressionFile = packageFiles
                    .FirstOrDefault(pf => pf.Name == ComponentExpressionPackageFile.FileName);

                if (componentFile != null)
                {
                    var reader = new ObjectXmlReader(componentFile.Stream);

                    var container = reader.ReadObject() as ObjectContainer<PComponent>;

                    this.Components.AddRange(container.Items);

                    // Binder Cache
                    foreach (PComponent c in container.Items)
                        CachingBinder(c);
                }

                if (expressionFile != null)
                {
                    var reader = new ObjectXmlReader(expressionFile.Stream);

                    var container = reader.ReadObject() as ObjectContainer<BindExpression>;

                    bindExpressions = container.Items;
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        private void CachingBinder(IBinderHostProvider provider)
        {
            foreach (PBinder item in provider.ProvideValue().Items)
                binderInfos[item.Guid] = item;
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
            this.Manifest.ReferencedModules.Items.Clear();
            packageFiles.Clear();

            // Update Referenced Modules
            Assembly coreAssembly = Assembly.GetAssembly(typeof(PComponent));

            foreach (var c in this.Components)
            {
                string name = c.GetType().Assembly.FullName;

                if (c is PFunction pFunc)
                    name = pFunc.FunctionInfo.DeclaringType.Assembly.FullName;

                if (name != coreAssembly.FullName)
                    this.Manifest.ReferencedModules.Items.SafeAdd(name);
            }

            // Set Manifest
            packageFiles.Add(
                new ManifestPackageFile(this.Manifest));

            // Set Screens
            foreach (var screen in Screens)
                packageFiles.Add(
                    new ScreenPackageFile(screen));

            // Set Components
            packageFiles.Add(
                new ComponentPackageFile(this.Components));

            packageFiles.Add(
                new ComponentExpressionPackageFile(this.Components));

            packageFiles.Add(
                new ComponentRendererPackageFile(this.Components));

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

        public PBinder GetComponentBinder(Guid guid)
        {
            if (BoolEx.Nomalize(binderInfos?.ContainsKey(guid)))
                return binderInfos[guid];

            return null;
        }

        public IEnumerable<BindExpression> GetBindExpressions()
        {
            if (bindExpressions == null)
                return Enumerable.Empty<BindExpression>();

            return bindExpressions;
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
            {
                RecentDB.AddFile(Path.GetFullPath(path));
            }

            return proj;
        }

        public static DXProject OpenDialog()
        {
            var fileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "DeXign 프로젝트 파일(*.dx)|*.dx"
            };

            bool result = (fileDialog.ShowDialog()).Nomalize();

            if (result)
            {
                var proj = DXProject.Open(fileDialog.FileName);

                if (!proj.CanOpen)
                    return null;

                return proj;
            }

            return null;
        }
        #endregion
    }
}