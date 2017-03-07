using System.IO;

using DeXign.Core;
using System;
using System.Windows;

namespace DeXign.IO
{
    internal abstract class ModelPackageFile<T> : PackageFile
        where T : PObject
    {
        public T Model { get; set; }

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
                var xmlWriter = new PModelXmlWriter(xmlStream);

                xmlWriter.WriteModel(this.Model);

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
