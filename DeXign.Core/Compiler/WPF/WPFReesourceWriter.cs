using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace DeXign.Core.Compiler
{
    internal class WPFResourceWriter : IDisposable
    {
        ResourceWriter writer;

        List<string> resourceNames;

        public WPFResourceWriter(Stream stream)
        {
            writer = new ResourceWriter(stream);
            resourceNames = new List<string>();
        }

        // directory: A/B/C...
        // fileName: C:\a.png
        public void AddImage(string fileName, string directory)
        {
            string name = Path.GetFileName(fileName);

            if (!string.IsNullOrWhiteSpace(directory))
                name = directory + "/" + name;

            name = name.ToLower();

            if (resourceNames.Contains(name))
                return;

            resourceNames.Add(name);
            writer.AddResource(name, new MemoryStream(File.ReadAllBytes(fileName)));
        }

        public void AddXaml(string name, string directory, string xaml)
        {
            if (!string.IsNullOrWhiteSpace(directory))
                name = directory + "/" + name;
            
            if (resourceNames.Contains(name))
                return;

            resourceNames.Add(name);

            writer.AddResource(name, xaml);
        }

        public void AddXaml(string name, string directory, byte[] xaml)
        {
            if (!string.IsNullOrWhiteSpace(directory))
                name = directory + "/" + name;

            if (resourceNames.Contains(name))
                return;

            resourceNames.Add(name);

            writer.AddResource(name, new MemoryStream(xaml));
        }

        public void Close()
        {
            writer.Generate();
            writer.Close();
        }

        public void Dispose()
        {
            if (writer == null)
                return;

            writer.Dispose();

            writer = null;
        }
    }
}
