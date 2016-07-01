using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PhotoStoryToBloomConverter.BloomModel;
using PhotoStoryToBloomConverter.PS3Model;

namespace PhotoStoryToBloomConverter
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new MainScreen().ShowDialog();
        }

        public static void ConvertToBloom(PhotoStoryProject project, string destinationFile, string bookName, IList<string> text)
        {
            var document = new BloomDocument(project, bookName, Path.GetDirectoryName(destinationFile));
            Ps3AndBloomSerializer.SerializeBloomHtml(document.ConvertToHtml(text), destinationFile);
        }

        //The assumption is that the wp3 archive only contains assets and a project.xml file. We convert the .xml file and copy the images and audio tracks.
        public static void CopyAssetsAndResources(string sourceFolderPath, string destinationFolderPath)
        {
            foreach (var filePath in Directory.EnumerateFiles(sourceFolderPath))
            {
                var filename = Path.GetFileName(filePath);
                if (filename.Equals("project.xml"))
                    continue;

                if (IsAudioFile(filename))
                {
                    Directory.CreateDirectory(Path.Combine(destinationFolderPath, BloomAudio.kAudioDirectory));
                    File.Copy(Path.Combine(sourceFolderPath, filename), Path.Combine(destinationFolderPath, BloomAudio.kAudioDirectory, filename));
                }
                else
                    File.Copy(Path.Combine(sourceFolderPath, filename), Path.Combine(destinationFolderPath, filename));
            }
        }

        public static void CopyBloomFiles(string destinationFolderPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourcename in assembly.GetManifestResourceNames())
            {
	            if (!resourcename.Contains("BloomBookResources"))
		            continue;

                //Resource names are of the form PhotoStoryToBloomConverter.BloomBookResources.Filename
                var components = resourcename.Split('.');
                var filename = string.Join(".", components.Skip(components.Length - 2));

                using (var stream = assembly.GetManifestResourceStream(resourcename))
                {
                    using (
                        var filestream = new FileStream(Path.Combine(destinationFolderPath, filename),
                            FileMode.CreateNew))
                    {
                        if (stream != null) stream.CopyTo(filestream);
                    }
                }
            }
        }

        private static bool IsAudioFile(string fileName)
        {
			return Path.GetExtension(fileName) == ".mp3" || Path.GetExtension(fileName) == ".wav" || Path.GetExtension(fileName) == ".wma";
        }
    }
}
