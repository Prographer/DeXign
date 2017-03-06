using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace DeXign.IO
{
    internal static class Package
    {
        /// <summary>
        /// 스트림 패키지 파일을 압축하고 압축된 스트림을 가져옵니다.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static Stream Packaging(params PackageFile[] files)
        {
            var ms = new MemoryStream();
            var za = new ZipArchive(ms, ZipArchiveMode.Create, true);
                
            foreach (var file in files)
            {
                if (file.IsEmpty)
                    continue;

                ZipArchiveEntry entry = za.CreateEntry(file.Name);
                Stream entryStream = entry.Open();

                file.Stream.Seek(0, SeekOrigin.Begin);
                file.Stream.CopyTo(entryStream);

                entryStream.Dispose();
            }

            za.Dispose();

            return ms;
        }

        /// <summary>
        /// 압축된 스트림 패키지 파일을 해석하여 열거형으로 가져옵니다.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IEnumerable<PackageFile> Unpackaging(Stream stream)
        {
            var za = new ZipArchive(stream, ZipArchiveMode.Read);
                
            foreach (ZipArchiveEntry entry in za.Entries)
            {
                Stream entryStream = entry.Open();
                var ms = new MemoryStream();

                entryStream.CopyTo(ms);
                entryStream.Dispose();

                ms.Seek(0, SeekOrigin.Begin);

                yield return new PackageFile(
                    entry.FullName, ms);
            }

            za.Dispose();
        }
    }
}