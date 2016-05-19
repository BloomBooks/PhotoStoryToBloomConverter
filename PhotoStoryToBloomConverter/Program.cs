using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using PhotoStoryToBloomConverter.BloomModel;
using PhotoStoryToBloomConverter.PS3Model;

namespace PhotoStoryToBloomConverter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine(
                    "usage: .\\PhotoStory3XmlToBloomHtml5Converter path/to/project.xml path/to/bloomBookDirectory bookName");
                return;
            }

            var projectPath = args[0];
            if (!Path.IsPathRooted(projectPath))
                projectPath = Path.Combine(Directory.GetCurrentDirectory(), projectPath);

            if (!File.Exists(projectPath))
            {
                Console.WriteLine("Unable to locate project file at {0}", projectPath);
                return;
            }

            var outputPath = args[1];
            if (!Path.IsPathRooted(outputPath))
                outputPath = Path.Combine(Directory.GetCurrentDirectory(), outputPath);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var bookName = args[2];
            var bookFolderPath = Path.Combine(outputPath, bookName);
            Directory.CreateDirectory(bookFolderPath);

            Console.WriteLine("Project path: {0}, Output path: {1}, Book name: {2}", projectPath, outputPath, bookName);
            Console.WriteLine("Book folder: {0}", bookFolderPath);
            var ps3Project = Ps3AndBloomSerializer.DeserializePhotoStoryXml(projectPath);
            ConvertToBloom(ps3Project, Path.Combine(bookFolderPath, string.Format("{0}.htm", bookName)), bookName);
            CopyAssetsAndResources(Path.GetDirectoryName(projectPath), bookFolderPath);
            CopyBloomFiles(bookFolderPath);
        }

        public static void ConvertToBloom(PhotoStoryProject project, string destinationFile, string bookName)
        {
            var document = BloomDocument.DefaultPhotoStoryConvertedBloomDocument(bookName);
            foreach (var unit in project.VisualUnits)
            {
                var image = unit.Image;
                var bloomImage = new BloomImage(image.Path, new Size {Height = image.Height, Width = image.Width});
                document.AddPage(BloomPage.DefaultBloomPageWithImage(bloomImage));
            }
            Ps3AndBloomSerializer.SerializeBloomHtml(document.ConvertToHtml(), destinationFile);
        }

        //The assumption is that the wp3 archive only contains assets and a project.xml file. We convert the .xml file and copy the images and audio tracks.
        public static void CopyAssetsAndResources(string sourceFolderPath, string destinationFolderPath)
        {
            Console.WriteLine("Copying files from directory {0} to directory {1}", sourceFolderPath,
                destinationFolderPath);
            foreach (var filePath in Directory.EnumerateFiles(sourceFolderPath))
            {
                var filename = Path.GetFileName(filePath);
                if (filename.Equals("project.xml")) continue;
                Console.WriteLine("Copying file from {0} to {1}", Path.Combine(sourceFolderPath, filename),
                    Path.Combine(destinationFolderPath, filename));
                File.Copy(Path.Combine(sourceFolderPath, filename), Path.Combine(destinationFolderPath, filename));
            }
        }

        public static void CopyBloomFiles(string destinationFolderPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourcename in assembly.GetManifestResourceNames())
            {
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
    }
}
