using System;
using System.Drawing;
using System.IO;
using PhotoStoryToBloomConverter.BloomModel;
using PhotoStoryToBloomConverter.PS3Model;

namespace PhotoStoryToBloomConverter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage: .\\PhotoStory3XmlToBloomHtml5Converter projectFilePath outputFilePath");
                return;
            }

            var projectPath = args[0];
            if (!Path.IsPathRooted(projectPath))
                projectPath = Path.Combine(Directory.GetCurrentDirectory(), projectPath);

            if (!File.Exists(projectPath))
            {
                Console.WriteLine($"Unable to locate project file at {projectPath}");
                return;
            }

            var outputPath = args[1];
            if (!Path.IsPathRooted(outputPath))
                outputPath = Path.Combine(Directory.GetCurrentDirectory(), outputPath);

            var enclosingDirectory = Path.GetDirectoryName(outputPath);
            if (enclosingDirectory == null)
            {
                Console.WriteLine($"Invalid output file path {outputPath}");
                return;
            }
            if (!Directory.Exists(enclosingDirectory))
                Directory.CreateDirectory(enclosingDirectory);

            var ps3Project = Ps3AndBloomSerializer.DeserializePhotoStoryXml(projectPath);
            ConvertToBloom(ps3Project, outputPath);
        }

        public static void ConvertToBloom(PhotoStoryProject project, string destinationFile)
        {
            var document = BloomDocument.DefaultPhotoStoryConvertedBloomDocument();
            foreach (var unit in project.VisualUnits)
            {
                var image = unit.Image;
                var bloomImage = new BloomImage(image.Path, new Size { Height = image.Height, Width = image.Width });
                document.AddPage(BloomPage.DefaultBloomPageWithImage(bloomImage));
            }
            Ps3AndBloomSerializer.SerializeBloomHtml(document.ConvertToHtml(), destinationFile);
        }
    }
}
