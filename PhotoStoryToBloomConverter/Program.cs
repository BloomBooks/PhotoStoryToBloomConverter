using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PhotoStoryToBloomConverter.BloomModel;
using PhotoStoryToBloomConverter.PS3Model;
using System.Diagnostics;

namespace PhotoStoryToBloomConverter
{

    public class Program
    {
        private static Boolean overwrite = false;
        private static Boolean batch = false;

        [STAThread]
        public static void Main(string[] args)
        {
            string projectPath = null;
            string collectionPath = null;
            string bloomPath = null;
            string projectName = null;
            string docxPath = null;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg == "-h" || arg == "-help" || arg == "-?" || arg == "?")
                {
                    DisplayUsage();
                    return;
                }
                if (arg == "-g")
                {
                    new MainScreen().ShowDialog();
                    return;
                }
                if (arg == "-b")
                {
                    batch = true;
                }
                else if (arg == "-f")
                {
                    overwrite = true;
                }
                else if (arg == "-pn")
                {
                    if(args.Length > i+1)
                    {
                        projectName = args[i + 1];
                    }
                    else
                    {
                        Console.WriteLine("Project name not set");
                        DisplayUsage();
                    }
                }
                else if (arg == "-t")
                {
                    if (args.Length > i + 1)
                    {
                        docxPath = args[i + 1];
                    }
                    else
                    {
                        Console.WriteLine("Word document path not set");
                        DisplayUsage();
                    }
                }
                else
                {
                    if (projectPath == null)
                        projectPath = arg;
                    else if (collectionPath == null)
                        collectionPath = arg;
                    else if (bloomPath == null)
                        bloomPath = arg;
                }
            }
            if (batch && projectPath != null)
            {
                //Reassign arguments
                var batchPath = projectPath;
                bloomPath = collectionPath;

                if (!Directory.Exists(batchPath))
                {
                    Console.WriteLine("Error: batchDirectoryPath does not exist.");
                    return;
                }
                if (!File.Exists(bloomPath))
                {
                    Console.WriteLine("Error: bloomAppPath does not exist.");
                    return;
                }
                BatchConvert(batchPath, bloomPath);
            }
            else if (!batch && projectPath != null && collectionPath != null && bloomPath != null)
            {
                if (!File.Exists(projectPath))
                {
                    Console.WriteLine("Error: projectXmlPath does not exist.");
                    return;
                }
                if (!File.Exists(collectionPath))
                {
                    Console.WriteLine("Error: bloomCollectionPath does not exist.");
                    return;
                }
                if (!File.Exists(bloomPath))
                {
                    Console.WriteLine("Error: bloomAppPath does not exist.");
                    return;
                }
                Convert(projectPath, Path.GetDirectoryName(collectionPath), projectName, docxPath, bloomPath);
            }
            else
            {
                DisplayUsage();
            }
        }

	    private static void DisplayUsage()
		{
            Console.WriteLine("usage: PhotoStoryToBloomConverter.exe projectXmlPath bloomCollectionPath bloomAppPath [-f] [-pn projectName] [-t narrativeDocxPath]");
			Console.WriteLine("       PhotoStoryToBloomConverter.exe -b batchDirectoryPath bloomAppPath [-f]");
            Console.WriteLine("       PhotoStoryToBloomConverter.exe -g");
		}

        public static void Convert(string projectXmlPath, string destinationFolder, string projectName, string docxPath, string bloomPath, PhotoStoryProject photoStoryProject = null, IList<string> extractedText = null)
	    {
            if (photoStoryProject == null)
            {
                photoStoryProject = Ps3AndBloomSerializer.DeserializePhotoStoryXml(projectXmlPath);
                if (string.IsNullOrEmpty(projectName))
                    projectName = photoStoryProject.GetProjectName();
            }

            var convertedProjectDirectory = Path.Combine(destinationFolder, projectName);
            if (Directory.Exists(convertedProjectDirectory) && !overwrite)
            {
                Console.WriteLine(string.Format("Error: A book already exists with the name {0}.", projectName), "projectName");
                return;
            }
            else if (Directory.Exists(convertedProjectDirectory) && overwrite)
            {
                DeleteAllFilesAndFoldersInDirectory(convertedProjectDirectory);
            }
            else
                Directory.CreateDirectory(convertedProjectDirectory);

            if(extractedText == null && docxPath != null)
            {
                TextExtractor.TryExtractText(docxPath, out extractedText);
                if (extractedText == null)
                    Console.WriteLine("Unable to extract text from {0}", docxPath);
            }

            //Three things needed for a bloom book: 
            //  book assets (images, narration audio, background audio)
            //  bloom book css and images
            //  the actual book, a generated html file built from the photostory project
			CopyAssetsAndResources(Path.GetDirectoryName(projectXmlPath), convertedProjectDirectory);
			CopyBloomFiles(convertedProjectDirectory);
			ConvertToBloom(photoStoryProject, Path.Combine(convertedProjectDirectory, string.Format("{0}.htm", projectName)), projectName, extractedText);

            //Process.Start(bloomPath, string.Format("hydrate --preset app --bookpath {0} --VernacularIsoCode en", convertedProjectDirectory));
            if(!batch)Console.WriteLine("Successfully converted {0}", projectName);
	    }

	    public static void BatchConvert(string directoryPath, string bloomExePath)
		{
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
				var matchingDocxFile = GetMatchingDocxFile(directoryPath, projectPath);
                Convert(projectXmlPath, outputDirectory, projectName, matchingDocxFile, bloomExePath, photoStoryProject);

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
            foreach (DirectoryInfo directory in new DirectoryInfo(directoryPath).GetDirectories())
                directory.Delete(true);
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

        //Pulls in all the gathered information for the poject and creates a single bloom book html file at destinationFile
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

                //Converting all .wav files to .ogg
	            if (AudioFileNeedsConversion(filename))
				{
					Directory.CreateDirectory(Path.Combine(destinationFolderPath, BloomAudio.kAudioDirectory));
					var newFileName = ConvertAudioFile(Path.Combine(sourceFolderPath, filename), Path.Combine(destinationFolderPath, BloomAudio.kAudioDirectory, Path.GetFileNameWithoutExtension(filename)));
					if (newFileName == null)
						throw new ApplicationException(string.Format("Unable to convert {0}.", Path.Combine(sourceFolderPath, filename)));
	            }
                //Audio files currently registered are .mp3 .wav .wma, going to store them all in a separate audio directory
                else if (IsAudioFile(filename))
                {
                    Directory.CreateDirectory(Path.Combine(destinationFolderPath, BloomAudio.kAudioDirectory));
                    File.Copy(Path.Combine(sourceFolderPath, filename), Path.Combine(destinationFolderPath, BloomAudio.kAudioDirectory, filename));
                }
                //Assuming these are our image assets
                else
                    File.Copy(Path.Combine(sourceFolderPath, filename), Path.Combine(destinationFolderPath, filename));
            }
        }

        public static void CopyBloomFiles(string destinationFolderPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourcename in assembly.GetManifestResourceNames())
            {
                //Resource names are of the form PhotoStoryToBloomConverter.BloomBookResources.Filename
	            if (!resourcename.Contains("BloomBookResources"))
		            continue;

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

	    private static bool AudioFileNeedsConversion(string fileName)
		{
			return Path.GetExtension(fileName) == ".wav";
		}

		private static string ConvertAudioFile(string sourcePath, string targetPathWithoutExtension)
		{
			return new OggEncoder().Encode(sourcePath, targetPathWithoutExtension);
		}

		private static bool IsAudioFile(string fileName)
		{
			return Path.GetExtension(fileName) == ".mp3" || Path.GetExtension(fileName) == ".wav" || Path.GetExtension(fileName) == ".wma";
		}
    }
}
