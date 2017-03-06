using System;
using System.IO;

namespace DeXign.IO
{
    internal class PackageFile : IDisposable
    {
        public string Name { get; protected set; }
        public Stream Stream { get; protected set; }

        public bool IsEmpty => (string.IsNullOrEmpty(Name) || Stream == null);

        private bool isDisposed;

        public PackageFile()
        {
        }

        public PackageFile(string name, Stream stream)
        {
            this.Name = name;
            this.Stream = stream;
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;

            Stream?.Dispose();
            Stream = null;

            OnDispose();
        }

        protected void OnDispose()
        {
        }
    }
}