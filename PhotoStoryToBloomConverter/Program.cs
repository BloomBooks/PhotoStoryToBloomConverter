using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoStoryToBloomConverter.BloomModel;
using PhotoStoryToBloomConverter.PS3Model;
using System.Diagnostics;

namespace PhotoStoryToBloomConverter
{

    public class Program
    {
        private static Boolean s_overwrite;
        private static Boolean s_batch;

        [STAThread]
        public static void Main(string[] args)
        {
            string projectPath = null;
            string collectionPath = null;
            string bloomPath = null;
            string projectName = null;
            string docxPath = null;
	        string docxDirectory = null;
	        string projectCode = null;

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
                    s_batch = true;
                }
                else if (arg == "-f")
                {
                    s_overwrite = true;
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
				else if (arg == "-td")
				{
					if (args.Length > i + 1)
					{
						docxDirectory = args[i + 1];
					}
					else
					{
						Console.WriteLine("Word document directory not set");
						DisplayUsage();
					}
				}
				else if (arg == "-c")
				{
					if (args.Length > i + 1)
					{
						projectCode = args[i + 1];
					}
					else
					{
						Console.WriteLine("Project code not set");
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
            if (s_batch && projectPath != null)
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
            else if (!s_batch && projectPath != null && collectionPath != null && bloomPath != null)
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
	            IEnumerable<string> docxPaths;
	            if (docxDirectory != null)
					docxPaths = GetMatchingDocxFiles(docxDirectory, projectCode);
	            else
		            docxPaths = new List<string> { docxPath };
				Convert(projectPath, Path.GetDirectoryName(collectionPath), projectName, docxPaths, bloomPath);
            }
            else
            {
                DisplayUsage();
            }
        }

	    private static void DisplayUsage()
		{
            Console.WriteLine("usage: PhotoStoryToBloomConverter.exe projectXmlPath bloomCollectionPath bloomAppPath [-f] [-pn projectName] [-t narrativeDocxPath | -td narrativeDocxDirectory -c projectCode]");
			Console.WriteLine("       PhotoStoryToBloomConverter.exe -b batchDirectoryPath bloomAppPath [-f]");
            Console.WriteLine("       PhotoStoryToBloomConverter.exe -g");
		}

        public static void Convert(string projectXmlPath, string destinationFolder, string projectName, IEnumerable<string> docxPaths, string bloomPath, PhotoStoryProject photoStoryProject = null)
	    {
			if (docxPaths == null)
				docxPaths = new List<string>();

            if (photoStoryProject == null)
            {
                photoStoryProject = Ps3AndBloomSerializer.DeserializePhotoStoryXml(projectXmlPath);
                if (string.IsNullOrEmpty(projectName))
                    projectName = photoStoryProject.GetProjectName();
            }

            var convertedProjectDirectory = Path.Combine(destinationFolder, projectName);
            if (Directory.Exists(convertedProjectDirectory) && !s_overwrite)
            {
                Console.WriteLine(string.Format("Error: A book already exists with the name {0}.", projectName), "projectName");
                return;
            }
            if (Directory.Exists(convertedProjectDirectory) && s_overwrite)
            {
                DeleteAllFilesAndFoldersInDirectory(convertedProjectDirectory);
            }
            else
                Directory.CreateDirectory(convertedProjectDirectory);

			var languageDictionary = new Dictionary<Language, IList<string>>();
	        foreach (var docxPath in docxPaths)
	        {
				IList<string> languageText;
		        var language = Path.GetFileNameWithoutExtension(docxPath).GetLanguageFromFileNameWithoutExtension();
				if (language != Language.Unknown && languageDictionary.ContainsKey(language))
					continue;
				if (TextExtractor.TryExtractText(docxPath, out languageText))
					languageDictionary.Add(language, languageText);
	        }

			var allLanguages = new List<List<KeyValuePair<Language, string>>>();
			for (int index = 0; index < languageDictionary[Language.English].Count; index++)
			{
				var list = new List<KeyValuePair<Language, string>>(6);
				foreach (var language in languageDictionary)
				{
					if (language.Value.Count > index)
						list.Add(new KeyValuePair<Language, string>(language.Key, language.Value[index]));
				}
				allLanguages.Add(list);
	        }

            //Three things needed for a bloom book: 
            //  book assets (images, narration audio, background audio)
            //  bloom book css and images
            //  the actual book, a generated html file built from the photostory project
			CopyAssetsAndResources(Path.GetDirectoryName(projectXmlPath), convertedProjectDirectory);
			ConvertToBloom(photoStoryProject, Path.Combine(convertedProjectDirectory, string.Format("{0}.htm", projectName)), projectName, allLanguages);

            var hydrationArguments = string.Format("hydrate --preset app --bookpath \"{0}\" --VernacularIsoCode en", convertedProjectDirectory);
	        bool hydrateSuccessful;
	        try
	        {
				using (var process = Process.Start(bloomPath, hydrationArguments))
				{
					process.WaitForExit();
					hydrateSuccessful = process.ExitCode == 0;
				}
	        }
	        catch
	        {
		        hydrateSuccessful = false;
	        }
			if (!hydrateSuccessful)
				Console.WriteLine("Unable to hydrate {0}", projectName);
	        else if (!s_batch)
		        Console.WriteLine("Successfully converted {0}", projectName);
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
				var projectFileNameWithoutExtension = Path.GetFileNameWithoutExtension(projectPath);
                if (string.IsNullOrWhiteSpace(projectName))
					projectName = projectFileNameWithoutExtension;

				var projectCode = projectFileNameWithoutExtension.Split(' ')[0];
				var matchingDocxFiles = GetMatchingDocxFiles(directoryPath, projectCode);
                Convert(projectXmlPath, outputDirectory, projectName, matchingDocxFiles, bloomExePath, photoStoryProject);

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

		private static IEnumerable<string> GetMatchingDocxFiles(string directoryPath, string projectCode)
		{
			var directoryInfo = new DirectoryInfo(directoryPath);
			var matchingFiles = directoryInfo.GetFiles(projectCode + "*.docx");
			return matchingFiles.Select(f => Path.Combine(directoryPath, f.Name));
		}

        //Pulls in all the gathered information for the project and creates a single bloom book html file at destinationFile
        public static void ConvertToBloom(PhotoStoryProject project, string destinationFile, string bookName, IList<List<KeyValuePair<Language, string>>> text)
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

	    private static bool AudioFileNeedsConversion(string fileName)
		{
			return Path.GetExtension(fileName) == ".wav";
		}

		private static string ConvertAudioFile(string sourcePath, string targetPathWithoutExtension)
		{
			return new Mp3Encoder().Encode(sourcePath, targetPathWithoutExtension);
		}

		private static bool IsAudioFile(string fileName)
		{
			return Path.GetExtension(fileName) == ".mp3" || Path.GetExtension(fileName) == ".wav" || Path.GetExtension(fileName) == ".wma";
		}
    }
}
