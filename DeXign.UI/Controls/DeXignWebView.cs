using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.IO;
using System.Resources;
using System.Reflection;
using WPFExtension;
using System;
using System.Collections.Generic;

using CefSharp;
using CefSharp.Wpf;

namespace DeXign.UI.Controls
{
    public class DeXignWebView : ContentControl
    {
        private static Dictionary<string, Assembly> assemblyCache;

        public static readonly DependencyProperty AddressProperty =
            DependencyHelper.Register();

        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        //public ChromiumWebBrowser NativeWebBrowser { get; }

        static DeXignWebView()
        {
            assemblyCache = new Dictionary<string, Assembly>();

            if (!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            File.WriteAllBytes("Resources\\cef.pak", Properties.Resources.cef);
            File.WriteAllBytes("Resources\\cef_100_percent.pak", Properties.Resources.cef_100_percent);
            File.WriteAllBytes("Resources\\cef_200_percent.pak", Properties.Resources.cef_200_percent);
            File.WriteAllBytes("Resources\\cef_extensions.pak", Properties.Resources.cef_extensions);
            File.WriteAllBytes("Resources\\devtools_resources.pak", Properties.Resources.devtools_resources);

            //Cef.Initialize(new CefSettings()
            //{
            //    ResourcesDirPath = Path.GetFullPath("Resources"),
            //    UserAgent = "Mozilla/5.0 (Linux; U; Android 2.1-update1; ko-kr; Nexus One Build/ERE27) AppleWebKit/530.17 (KHTML, like Gecko) Version/4.0 Mobile Safari/530.17"
            //});
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (assemblyCache.TryGetValue(args.Name, out Assembly assm))
            {
                return assm;
            }

            return null;
        }

        public DeXignWebView()
        {
            var wb = new ChromiumWebBrowser();
            
            this.Content = wb;

            var b = new Binding(DeXignWebView.AddressProperty.Name)
            {
                Source = this,
                Mode = BindingMode.TwoWay
            };

            BindingOperations.SetBinding(wb, ChromiumWebBrowser.AddressProperty, b);
        }
    }
}
