using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;

using DeXign.Extension;
using DeXign.Core.Logic;
using DeXign.Core.Controls;

using Microsoft.CSharp;

namespace DeXign.Core.Compiler
{
    internal class WPFCompiler : BaseCompilerService
    {
        private static string[] LibraryDependency =
            new[]
            {
                "System.dll",
                "System.Xml.dll",
                "System.Xaml.dll",
                "System.Windows.dll",
                "WindowsBase.dll",
                "PresentationCore.dll",
                "PresentationFramework.dll",

                "DeXign.UI.dll",
                "WPFExtension.dll"
            };

        private static string[] CSFiles = 
            new[]
            {
                "DXApp.cs",
                "DXAppWindow.cs",
                "DXAssemblyInfo.cs",
                "DXMain.cs"
            };

        private static Dictionary<string, string> CodeResources;

        static WPFCompiler()
        {
            // pre-load
            WPFCompiler.CodeResources = new Dictionary<string, string>();

            foreach (string csFile in CSFiles)
                WPFCompiler.CodeResources[csFile] = GetTextResource($"Resources/{csFile}");
        }

        public WPFCompiler()
        {
            this.Platform = Platform.Window;
        }

        #region [ Compile ]
        public override DXCompileResult Compile(DXCompileOption option, PContentPage[] screens, PBinderHost[] hosts)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            // Result
            var result = new DXCompileResult(option);

            // Generator
            WPFLayoutGenerator layoutGenerator = CreateLayoutGenerator(option, screens);
            CSharpGenerator logicGenerator = CreateLogicGenerator(layoutGenerator, hosts);

            // CS Builder
            var csBuilder = new WPFCodeBuilder(option, screens, hosts);

            // CodeDom
            var provider = new CSharpCodeProvider();
            var compileParam = new CompilerParameters()
            {
                GenerateExecutable = true,
                GenerateInMemory = false,
                TreatWarningsAsErrors = false,
#if DEBUG
                IncludeDebugInformation = false
#else
                IncludeDebugInformation = true
#endif
            };

            // Add WPF Referenced Assembly
            Assembly[] dependencyLibs = GetReferencedAssemblyNames(screens, hosts).ToArray();

            AddReferencedAssemblies(compileParam, dependencyLibs);

            // 임시 파일 경로
            string tempIconPath = compileParam.TempFiles.AddExtension("ico");
            string tempResFileName = Path.Combine(Path.GetTempPath(), $"{option.ApplicationName}.g.resources");
            //                              기본 디렉터리 / Build / 어플리케이션이름 / 플랫폼 
            string directory = Path.Combine(option.Directory, "Build", option.ApplicationName, option.TargetPlatform.ToString());
            string exePath = Path.Combine(directory, $"{option.ApplicationName}.exe");

            compileParam.TempFiles.AddFile(tempResFileName, false);

            // 출력 폴더 생성
            DirectoryEx.CreateDirectory(directory);

            // Generate Native Code
            string[] screensXaml = layoutGenerator.Generate().ToArray();
            var csSources = new List<string>();

            var d = logicGenerator.Generate().ToArray();

            foreach (string cs in WPFCompiler.CodeResources.Values)
            {
                csSources.Add(csBuilder.Build(cs));
            }
            
            // 리소스 생성
            if (provider.Supports(GeneratorSupport.Resources))
            {
                using (var fs = File.Create(tempResFileName))
                {
                    var res = new WPFResourceWriter(fs);

                    // 이미지 리소스 추가
                    foreach (string img in layoutGenerator.Images)
                        res.AddImage(img, "");

                    // 레이아웃 xaml 추가
                    for (int i = 0; i < screens.Length; i++)
                        res.AddXaml($"{screens[i].GetPageName()}.xaml", "", screensXaml[i]);

                    res.Close();
                }

                compileParam.EmbeddedResources.Add(tempResFileName);
            }
            
            // 임시 아이콘 생성
            Stream iconStream = GetStreamResource("Resources/IconLogo.ico");
            byte[] iconBin = new byte[iconStream.Length];

            iconStream.Read(iconBin, 0, iconBin.Length);

            File.WriteAllBytes(tempIconPath, iconBin);
            
            // 출력 및 컴파일 커맨드라인 설정
            compileParam.OutputAssembly = exePath;
            compileParam.CompilerOptions = $"/target:winexe /win32icon:{tempIconPath}";

            // Compile Binary
            CompilerResults compileResult = provider.CompileAssemblyFromSource(compileParam, csSources.ToArray());
            compileParam.TempFiles.Delete();

            // 컴파일 시간 기록
            sw.Stop();
            result.Elapsed = sw.Elapsed;

            if (compileResult.Errors.Count > 0)
            {
                result.Errors.AddRange(compileResult.Errors.Cast<object>());
            }
            else
            {
                result.Outputs.Add(exePath);

                // Referenced DLL Export
                foreach (string assemblyFileName in compileParam.ReferencedAssemblies)
                {
                    if (File.Exists(assemblyFileName) &&
                        assemblyFileName.StartsWith(Environment.CurrentDirectory))
                    {
                        // DLL 복사
                        File.Copy(
                            assemblyFileName,
                            Path.Combine(directory, Path.GetFileName(assemblyFileName)),
                            true);

                        result.Outputs.Add(assemblyFileName);
                    }
                }

                result.IsSuccess = true;
            }

            return result;
        }
        #endregion

        #region [ Referenced Assembly ]
        private static IEnumerable<Assembly> GetReferencedAssemblyNames(PContentPage[] screens, PBinderHost[] components)
        {
            // 확장 모듈에서 생성된 객체인경우 어셈블리 가져옴

            List<Assembly> assemblies = screens.Select(s => s.GetType().Assembly).ToList();
            
            foreach (PBinderHost comp in components)
            {
                foreach (IBinderHost host in BinderHelper.FindHostNodes(comp))
                {
                    if (host is PFunction func)
                    {
                        assemblies.Add(func.FunctionInfo.DeclaringType.Assembly);
                    }
                    else if (host is PTrigger trigger)
                    {
                        assemblies.Add(trigger.EventInfo.DeclaringType.Assembly);
                    }
                }
            }

            return assemblies
                .Where(ass => !ass.GetName().Name.AnyEquals("DeXign.Core"));
        }

        private static void AddReferencedAssemblies(CompilerParameters param, params Assembly[] assemblies)
        {
            foreach (string assembly in LibraryDependency)
            {
                string location = AssemblyEx.GetAssemblyLocation(assembly);
                
                if (File.Exists(location))
                    param.ReferencedAssemblies.Add(location);
            }

            // 외부 라이브러리
            foreach (Assembly assm in assemblies)
            {
                param.ReferencedAssemblies.Add(assm.Location);
            }
        }
        #endregion

        #region [ Generator ]
        private static CSharpGenerator CreateLogicGenerator(WPFLayoutGenerator layoutGenerator, PBinderHost[] hosts)
        {
            var logicUnit = new LogicGeneratorUnit(hosts);

            // Logic Generator
            return new CSharpGenerator(
                layoutGenerator.NameContainer,
                logicUnit,
                layoutGenerator.Manifest,
                layoutGenerator.AssemblyInfo);
        }

        private static WPFLayoutGenerator CreateLayoutGenerator(DXCompileOption option, PContentPage[] screens)
        {
            var layoutUnit = new LayoutGeneratorUnit(screens);
            
            var assemblyInfo = new CodeGeneratorAssemblyInfo()
            {
                Title = option.ApplicationName
            };

            var manifest = new CodeGeneratorManifest()
            {
                RootNamespace = option.RootNamespace,
                ApplicationName = option.ApplicationName
            };

            // Layout Generator
            return new WPFLayoutGenerator(
                layoutUnit,
                manifest,
                assemblyInfo);
        }
        #endregion

        #region [ Resource ]
        private static Stream GetStreamResource(string path)
        {
            string uri = BuildResourceUri(path);

            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(uri);
        }

        private static string GetTextResource(string path)
        {
            Stream s = GetStreamResource(path);

            using (var sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }

        private static string BuildResourceUri(string path)
        {
            return $"DeXign.Core.Compiler.WPF.{path.Replace('/', '.')}";
        }
        #endregion
    }
}
