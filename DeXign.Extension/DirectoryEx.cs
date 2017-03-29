using System.IO;
using System.Linq;

namespace DeXign.Extension
{
    public static class DirectoryEx
    {
        public static void Create(string directory)
        {
            string[] seg = directory.Split('\\');

            for (int i = 0; i < seg.Length; i++)
            {
                string path = string.Join("\\", seg.Take(i + 1));

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }
    }
}
