using System;
using System.IO;
using Shell32;

namespace CABExtracter
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ExpandCabFile(Path.Combine(Directory.GetCurrentDirectory(), args[0]), Path.Combine(Directory.GetCurrentDirectory(), args[1]));
        }

        /// <summary>
        /// Takes in the absolute paths for a .cab archive and outputs the contained files into a folder allowing easier access to the files.
        /// </summary>
        /// <param name="cabFilePath">The absolute path to the cab archive to be extracted from</param>
        /// <param name="destinationDirectoryPath">The absolute path to the destination directory for extracted files</param>
        private static void ExpandCabFile(string cabFilePath, string destinationDirectoryPath)
        {
            var shell = new Shell();
            if (!Path.GetExtension(cabFilePath).Equals("cab"))
            {
                var newCabFilePath = Path.ChangeExtension(cabFilePath, "cab");
                File.Move(cabFilePath, newCabFilePath);
                cabFilePath = newCabFilePath;
            }

            if (!Directory.Exists(destinationDirectoryPath))
                Directory.CreateDirectory(destinationDirectoryPath);
            var fldr = shell.NameSpace(destinationDirectoryPath);
            foreach (FolderItem item in shell.NameSpace(cabFilePath).Items()) fldr.CopyHere(item, 0);
        }
    }
}
