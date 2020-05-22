using System.IO;

namespace WebApplication.Utilities
{
    /// <summary>
    /// Utilities for files/directories work.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Copy directory.
        /// </summary>
        public static void CopyDir(string dirFrom, string dirTo)
        {
            // based on https://stackoverflow.com/a/8022011
            var dirFromLength = dirFrom.Length + 1;

            foreach (string dir in Directory.GetDirectories(dirFrom, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(Path.Combine(dirTo, dir.Substring(dirFromLength)));
            }

            foreach (string fileName in Directory.GetFiles(dirFrom, "*", SearchOption.AllDirectories))
            {
                File.Copy(fileName, Path.Combine(dirTo, fileName.Substring(dirFromLength)));
            }
        }
    }
}
