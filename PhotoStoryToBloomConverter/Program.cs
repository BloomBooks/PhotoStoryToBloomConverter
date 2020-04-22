using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoStoryToBloomConverter
{

	public class Program
	{
		// Do some special formatting/output for Story Producer App
		public static bool SpAppOutput;

		private static bool s_overwrite;
		private static bool s_batch;
		private static bool s_alsoCreateZippedOutput;

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
						Console.WriteLine(@"Project name not set");
						DisplayUsage();
					}
				}
				// Originally, this was -r for "reference" but morphed into -sp for "Story Producer App".
				// We're leaving the old -r for backwards compatibility.
				else if (arg == "-r" || arg == "-sp")
				{
					SpAppOutput = true;
				}
				else if (arg == "-t")
				{
					if (args.Length > i + 1)
					{
						docxPath = args[i + 1];
					}
					else
					{
						Console.WriteLine(@"Word document path not set");
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
						Console.WriteLine(@"Word document directory not set");
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
						Console.WriteLine(@"Project code not set");
						DisplayUsage();
					}
				}
				else if (arg == "-z")
				{
					s_alsoCreateZippedOutput = true;
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
					Console.WriteLine(@"Error: batchDirectoryPath does not exist.");
					return;
				}
				if (!File.Exists(bloomPath))
				{
					Console.WriteLine(@"Error: bloomAppPath does not exist.");
					return;
				}
				BatchConvert(batchPath, bloomPath);

				Console.WriteLine(@"Press any key to close.");
				Console.ReadLine();
			}
			else if (!s_batch && projectPath != null && collectionPath != null && bloomPath != null)
			{
				if (!File.Exists(projectPath))
				{
					Console.WriteLine(@"Error: projectXmlPath does not exist.");
					return;
				}
				if (!File.Exists(collectionPath))
				{
					Console.WriteLine(@"Error: bloomCollectionPath does not exist.");
					return;
				}
				if (!File.Exists(bloomPath))
				{
					Console.WriteLine(@"Error: bloomAppPath does not exist.");
					return;
				}
				IEnumerable<string> docxPaths;
				if (docxDirectory != null)
					docxPaths = GetMatchingDocxFiles(docxDirectory, projectCode);
				else
					docxPaths = new List<string> { docxPath };
				var project = new Project(projectPath);
				project.Convert(Path.GetDirectoryName(collectionPath), projectName, null, docxPaths, bloomPath, s_overwrite, alsoZip: s_alsoCreateZippedOutput);

				Console.WriteLine(@"Press any key to close.");
				Console.ReadLine();
			}
			else
			{
				DisplayUsage();
			}
		}

		private static void DisplayUsage()
		{
			Console.WriteLine(@"options:");
			Console.WriteLine(@"	-f: force overwrite; -sp: Story Producer App output; -z: also produce .bloom output");
			Console.WriteLine();
			Console.WriteLine(@"typical use case is batch (-b) processing:");
			Console.WriteLine(@"	PhotoStoryToBloomConverter.exe -b batchSourceDirectoryPath bloomExePath [-f] [-sp] [-z]");
			Console.WriteLine(@"		Output will be in batchSourceDirectoryPath/Batch Conversion Output");
			Console.WriteLine();
			Console.WriteLine(@"single conversion (not recommended... much much less tested):");
			Console.WriteLine(@"	PhotoStoryToBloomConverter.exe projectXmlPath bloomCollectionPath bloomExePath [-f] [-pn projectName] [-t narrativeDocxPath | -td narrativeDocxDirectory -c projectCode]");
		}

		public static void BatchConvert(string directoryPath, string bloomExePath)
		{
			var outputDirectory = Path.Combine(directoryPath, "Batch Conversion Output");
			Directory.CreateDirectory(outputDirectory);

			var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tempFolder);
			var filesToProcess = Directory.EnumerateFiles(directoryPath, "*.wp3").Union(Directory.EnumerateFiles(directoryPath, "*.cab")).ToList();
			Console.WriteLine($@"Found {filesToProcess.Count} files to process in this directory...");
			int successCount = 0;
			int failureCount = 0;
			foreach (var projectPath in filesToProcess)
			{
				CABExtracter.Program.ExpandCabFile(projectPath, tempFolder);

				var projectXmlPath = Path.Combine(tempFolder, "project.xml");
				var photoStoryProject = Ps3AndBloomSerializer.DeserializePhotoStoryXml(projectXmlPath);
				var projectName = photoStoryProject.GetProjectName();
				var projectFileNameWithoutExtension = Path.GetFileNameWithoutExtension(projectPath);

				var projectCode = projectFileNameWithoutExtension.Split(' ')[0];

				if (string.IsNullOrWhiteSpace(projectName))
				{
					Console.WriteLine(@"WARNING: Could not get project name from Photo Story project");
					projectName = projectFileNameWithoutExtension;
				}
				else
				{
					if (!projectName.StartsWith(projectCode))
						projectName = $"{projectCode} {projectName}";
				}

				var matchingDocxFiles = GetMatchingDocxFiles(directoryPath, projectCode);
				var project = new Project(projectXmlPath);
				if (project.Convert(outputDirectory, projectName, projectCode, matchingDocxFiles, bloomExePath, s_overwrite, photoStoryProject, s_alsoCreateZippedOutput))
					successCount++;
				else
					failureCount++;

				DeleteAllFilesAndFoldersInDirectory(tempFolder);
			}

			Directory.Delete(tempFolder);

			Console.WriteLine();
			if (successCount > 0)
				Console.WriteLine($@"Successfully processed {successCount} files.");
			if (failureCount > 0)
				Console.WriteLine($@"Failed to process {failureCount} files.");
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
