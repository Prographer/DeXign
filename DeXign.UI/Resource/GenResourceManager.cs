using System.Collections;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Markup;

namespace DeXign.UI
{
    public static class GenResourceManager
    {
        public static ResourceDictionary LoadResourceDictionary(string name)
        {
            var s = GetEntry(name) as Stream;
            var reader = new XamlReader();

            return (ResourceDictionary)reader.LoadAsync(s);
            
            //Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);
        }

        public static object LoadXaml(string name)
        {
            string xaml = GetXaml(name);

            if (string.IsNullOrEmpty(xaml))
                return null;

            return XamlReader.Parse(xaml);
        }

        public static string GetXaml(string name)
        {
            object data = GetEntry(name);
            
            if (data is string)
                return (string)data;

            if (data is Stream stream)
                return new StreamReader(stream).ReadToEnd();

            return null;
        }

        private static object GetEntry(string name)
        {
            var assm = Assembly.GetEntryAssembly();
            string genResourceName = assm.GetManifestResourceNames()[0];
            
            Stream stream = assm.GetManifestResourceStream(genResourceName);

            var resourceReader = new ResourceReader(stream);

            foreach (DictionaryEntry entry in resourceReader)
            {
                if (entry.Key.ToString() == name)
                {
                    return entry.Value;
                }
            }

            return null;
        }
    }
}
