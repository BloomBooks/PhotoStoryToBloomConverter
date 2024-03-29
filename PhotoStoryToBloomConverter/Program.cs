using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PhotoStoryToBloomConverter
{

	public class Program
	{
		// Do some special formatting/output for Story Producer App
		public static bool SpAppOutput;

		public static string PrimaryOutputLanguage = "en";

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
						docxPath = args[++i];
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
						docxDirectory = args[++i];
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
				else if (arg == "-lang")
				{
					if (args.Length > i + 1)
					{
						PrimaryOutputLanguage = args[i + 1];
						i++;
					}
					else
					{
						Console.WriteLine(@"Primary output language code not set");
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
			Console.WriteLine($"Converter version {Assembly.GetExecutingAssembly().GetName().Version}");
			Console.WriteLine($"Built on {GetBuiltOnDate()}");
			Console.WriteLine();
			Console.WriteLine($"Primary output language is {PrimaryOutputLanguage}");
			Console.WriteLine();
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
			}
			else
			{
				DisplayUsage();
			}
			Console.WriteLine(@"Press any key to close.");
			Console.ReadLine();
		}

		private static void DisplayUsage()
		{
			Console.WriteLine(@"options:");
			Console.WriteLine(@"	-f: force overwrite; -sp: Story Producer App output; -z: also produce .bloom output;");
			Console.WriteLine(@"	-lang: primary output language code (default is en)");
			Console.WriteLine();
			Console.WriteLine(@"typical use case is batch (-b) processing:");
			Console.WriteLine(@"	PhotoStoryToBloomConverter.exe -b batchSourceDirectoryPath bloomExePath [-f] [-sp] [-z] [-lang primaryOutputLanguageCode]");
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
			var wp3Files = Directory.EnumerateFiles(directoryPath, "*.wp3");
			var cabFiles = Directory.EnumerateFiles(directoryPath, "*.cab");
			var filesToProcess = wp3Files.Union(cabFiles).ToList();
			Console.WriteLine($@"Found {filesToProcess.Count} stories to process in this directory...");
			Console.WriteLine($"   ({wp3Files.Count()} .wp3 files and {cabFiles.Count()} .cab files)");
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
				Console.WriteLine();
				Console.WriteLine($"Processing {Path.GetFileName(projectPath)}...");
				Console.WriteLine($"   with docx file(s): {GetFileNameListAsString(matchingDocxFiles)}");
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
				Console.WriteLine($"Successfully processed {successCount} stories.");
			if (failureCount > 0)
				Console.WriteLine($"Failed to process {failureCount} stories.");
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

		private static string GetFileNameListAsString(IEnumerable<string> pathList)
		{
			var sb = new StringBuilder();
			foreach (var path in pathList)
				sb.Append(Path.GetFileName(path) + ", ");
			sb.Remove(sb.Length - 2, 2);
			if (pathList.Count() > 1)
			{
				sb.Insert(0, "[");
				sb.Append("]");
			}
			return sb.ToString();
		}

		private static string GetBuiltOnDate()
		{
			var fileString = Assembly.GetExecutingAssembly().CodeBase;

			if (string.IsNullOrEmpty(fileString))
				return fileString;

			var prefix = Uri.UriSchemeFile + ":";

			if (!fileString.StartsWith(prefix))
				return fileString;

			var file = fileString.Substring(prefix.Length);
			// Trim any number of beginning slashes
			file = file.TrimStart('/');

			var fi = new FileInfo(file);

			// Use UTC for calculation of build-on-date so that we get the same date regardless
			// of timezone setting.
			return fi.CreationTimeUtc.ToString("dd-MMM-yyyy");
		}
	}
}
