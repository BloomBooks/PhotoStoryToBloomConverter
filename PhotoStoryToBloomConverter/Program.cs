using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

				Console.WriteLine("Press any key to close.");
				Console.ReadLine();
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
				var project = new Project(projectPath);
	            project.Convert(Path.GetDirectoryName(collectionPath), projectName, docxPaths, bloomPath, s_overwrite);

				Console.WriteLine("Press any key to close.");
				Console.ReadLine();
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

	    public static void BatchConvert(string directoryPath, string bloomExePath)
		{
			var outputDirectory = Path.Combine(directoryPath, "Batch Conversion Output");
			Directory.CreateDirectory(outputDirectory);

			var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tempFolder);
		    var filesToProcess = Directory.EnumerateFiles(directoryPath, "*.wp3").Union(Directory.EnumerateFiles(directoryPath, "*.cab")).ToList();
			Console.WriteLine("Found {0} files to process in this directory...", filesToProcess.Count);
			int successCount = 0;
			int failureCount = 0;
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

//				if (!(projectCode == "001"/* || projectCode == "002" || projectCode == "003"*/))
//				{
//					DeleteAllFilesAndFoldersInDirectory(tempFolder);
//					return;
//					//continue;
//				}

				var matchingDocxFiles = GetMatchingDocxFiles(directoryPath, projectCode);
				var project = new Project(projectXmlPath);
				if (project.Convert(outputDirectory, projectName, matchingDocxFiles, bloomExePath, s_overwrite, photoStoryProject))
					successCount++;
				else
					failureCount++;

				DeleteAllFilesAndFoldersInDirectory(tempFolder);
			}

			Directory.Delete(tempFolder);

		    Console.WriteLine();
			if (successCount > 0)
				Console.WriteLine("Successfully processed {0} files.", successCount);
			if (failureCount > 0)
				Console.WriteLine("Failed to process {0} files.", failureCount);
			Console.WriteLine();
		}

		public static void DeleteAllFilesAndFoldersInDirectory(string directoryPath)
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
    }
}
