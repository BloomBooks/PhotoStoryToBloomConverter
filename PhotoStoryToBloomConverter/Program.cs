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
	        if (args.Length == 0)
	        {
		        new MainScreen().ShowDialog();
		        return;
	        }

	        if (args[0] == "-h" || args[0] == "-help" || args[0] == "-?" || args[0] == "?")
	        {
		        DisplayHelp();
		        return;
	        }
	        try
	        {
		        if (args[0] == "-b")
		        {
			        BatchConvert(args[1]);
			        return;
		        }

		        string projectName = null;
				string docxPath = null;
		        if (args.Length > 2 && args[2] == "-pn")
		        {
					projectName = args[3];
					if (args.Length > 4 && args[4] == "-t")
						docxPath = args[5];
		        }
		        else if (args.Length > 2 && args[2] == "-t")
			        docxPath = args[3];
		        Convert(args[0], args[1], projectName, null, null, docxPath);
	        }
	        catch (ArgumentException ae)
	        {
		        Console.WriteLine();
		        Console.WriteLine(ae.Message);
		        Console.WriteLine();
	        }
	        catch (Exception e)
	        {
		        Console.WriteLine();
		        Console.WriteLine("An unexpected error occurred: ");
		        Console.WriteLine(e.Message);
		        Console.WriteLine();
	        }
        }

	    private static void DisplayHelp()
		{
			Console.WriteLine("Single conversion mode arguments:");
			Console.WriteLine("  path the project.xml file");
			Console.WriteLine("  path to bloom collection file");
			Console.WriteLine("  (optional) -pn => project name (if this is not provided, we will attempt to extract it from the project)");
			Console.WriteLine("  (optional) -t => path to .docx file containing text to extract");
			Console.WriteLine();
			Console.WriteLine("-b => batch mode (must be the first argument)");
		    Console.WriteLine();
			Console.WriteLine("Batch mode arguments:");
			Console.WriteLine("  path to directory containing wp3 files and docx files");
		}

	    public static void Convert(string projectXmlPath, string bloomCollectionPath, string projectName = null, PhotoStoryProject photoStoryProject = null, IList<string> extractedText = null, string docxPath = null)
	    {
		    if (string.IsNullOrEmpty(projectXmlPath))
				throw new ArgumentException("Project xml path must be provided.", "projectXmlPath");
			if (!File.Exists(projectXmlPath))
				throw new ArgumentException("The project does not exist.", "projectXmlPath");

		    if (photoStoryProject == null)
		    {
				photoStoryProject = Ps3AndBloomSerializer.DeserializePhotoStoryXml(projectXmlPath);
			    if (projectName == null)
				    projectName = photoStoryProject.GetProjectName();
			}
			else if (string.IsNullOrEmpty(projectName))
				throw new ArgumentException("Project name must be provided.", "projectName");

			if (string.IsNullOrEmpty(bloomCollectionPath))
				throw new ArgumentException("Bloom collection path must be provided.", "bloomCollectionPath");
			if (!File.Exists(bloomCollectionPath))
				throw new ArgumentException("The bloom file does not exist.", "bloomCollectionPath");
			var convertedProjectDirectory = Path.Combine(Path.GetDirectoryName(bloomCollectionPath), projectName);
			if (Directory.Exists(convertedProjectDirectory))
				throw new ArgumentException(string.Format("A book already exists with the name {0} in that collection.", projectName), "projectName");

		    if (extractedText == null && docxPath != null)
		    {
			    TextExtractor.TryExtractText(docxPath, out extractedText);
				if (extractedText == null)
					Console.WriteLine("Unable to extract text.");
		    }

		    Directory.CreateDirectory(convertedProjectDirectory);

			CopyAssetsAndResources(Path.GetDirectoryName(projectXmlPath), convertedProjectDirectory);
			ConvertToBloom(photoStoryProject, Path.Combine(convertedProjectDirectory, string.Format("{0}.htm", projectName)), projectName, extractedText);
			CopyBloomFiles(convertedProjectDirectory);
	    }

	    public static void BatchConvert(string directoryPath)
		{
			if (directoryPath.Length == 0 || !Directory.Exists(directoryPath))
				throw new ArgumentException("Please select an existing directory.", "directoryPath");

			var outputDirectory = Path.Combine(directoryPath, "Batch Conversion Output");
			Directory.CreateDirectory(outputDirectory);

			var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tempFolder);
		    var filesToProcess = Directory.EnumerateFiles(directoryPath, "*.wp3").Union(Directory.EnumerateFiles(directoryPath, "*.cab")).ToList();
			Console.WriteLine("Found {0} files to process in this directory...", filesToProcess.Count);
			foreach (var projectPath in filesToProcess)
			{
				CABExtracter.Program.ExpandCabFile(projectPath, tempFolder);

				var projectXmlPath = Path.Combine(tempFolder, "project.xml");

				var photoStoryProject = Ps3AndBloomSerializer.DeserializePhotoStoryXml(projectXmlPath);
				var projectName = photoStoryProject.GetProjectName();
				if (string.IsNullOrWhiteSpace(projectName))
					projectName = Path.GetFileNameWithoutExtension(projectPath);
				var convertedProjectDirectory = Path.Combine(outputDirectory, projectName);
				Directory.CreateDirectory(convertedProjectDirectory);

				var matchingDocxFile = GetMatchingDocxFile(directoryPath, projectPath);
				IList<string> text = null;
				if (matchingDocxFile != null)
					TextExtractor.TryExtractText(matchingDocxFile, out text);

				if (text == null)
					Console.WriteLine("Could not extract text for {0}", projectName);

				CopyAssetsAndResources(Path.GetDirectoryName(projectXmlPath), convertedProjectDirectory);
				ConvertToBloom(photoStoryProject, Path.Combine(convertedProjectDirectory, string.Format("{0}.htm", projectName)), projectName, text);
				CopyBloomFiles(convertedProjectDirectory);

				DeleteAllFilesAndFoldersInDirectory(tempFolder);

				Console.Write(".");
			}

			Directory.Delete(tempFolder);

		    Console.WriteLine();
			Console.WriteLine("Successfully processed {0} files.", filesToProcess.Count);
		}

		private static void DeleteAllFilesAndFoldersInDirectory(string directoryPath)
		{
			foreach (FileInfo file in new DirectoryInfo(directoryPath).GetFiles())
				file.Delete();
		}

		private static string GetMatchingDocxFile(string directoryPath, string wp3FilePath)
		{
			var wp3Name = Path.GetFileNameWithoutExtension(wp3FilePath);
			var wp3NameCode = wp3Name.Split(' ')[0];

			var directoryInfo = new DirectoryInfo(directoryPath);
			var matchingFiles = directoryInfo.GetFiles(wp3NameCode + "*.docx");
			if (matchingFiles.Length == 1)
				return matchingFiles[0].FullName;
			return null;
		}

        public static void ConvertToBloom(PhotoStoryProject project, string destinationFile, string bookName, IList<string> text)
        {
            var document = new BloomDocument(project, bookName, Path.GetDirectoryName(destinationFile), text);
            Ps3AndBloomSerializer.SerializeBloomHtml(document.ConvertToHtml(), destinationFile);
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
                //Don't copy if it is a credits or cover image, we won't use them (but do extract any credits information)
                else if (!CreditsExtractor.imageIsCreditsOrCover(Path.Combine(sourceFolderPath, filename)))
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
