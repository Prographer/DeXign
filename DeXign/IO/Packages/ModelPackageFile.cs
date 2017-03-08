using System;
using System.IO;
using System.Windows;

namespace DeXign.IO
{
    internal abstract class ModelPackageFile<T> : PackageFile
    {
        public T Model { get; set; }

        public ModelPackageFile()
        {
        }

        public ModelPackageFile(T model)
        {
            this.Model = model;

            CreateStream();
        }

        protected virtual void CreateStream()
        {
            try
            {
                var xmlStream = new MemoryStream();
                var xmlWriter = new ObjectXmlWriter(xmlStream);

                xmlWriter.WriteObject(this.Model);

                this.Stream = xmlStream;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }
    }
}
