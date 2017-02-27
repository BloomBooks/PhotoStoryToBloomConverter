using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PhotoStoryToBloomConverter.BloomModel;
using PhotoStoryToBloomConverter.PS3Model;
using SIL.Extensions;

namespace PhotoStoryToBloomConverter
{
	class Project
	{
		private readonly string _projectXmlPath;
		private readonly AudioHelper _audioHelper = new AudioHelper();

		public Project(string projectXmlPath)
		{
			_projectXmlPath = projectXmlPath;
		}

		public void Convert(string destinationFolder, string projectName, IEnumerable<string> docxPaths, string bloomPath, bool overwrite, PhotoStoryProject photoStoryProject = null)
		{
			if (docxPaths == null)
				docxPaths = new List<string>();

			if (photoStoryProject == null)
			{
				photoStoryProject = Ps3AndBloomSerializer.DeserializePhotoStoryXml(_projectXmlPath);
				if (String.IsNullOrEmpty(projectName))
					projectName = photoStoryProject.GetProjectName();
			}

			var convertedProjectDirectory = Path.Combine(destinationFolder, projectName);
			if (Directory.Exists(convertedProjectDirectory) && !overwrite)
			{
				Console.WriteLine(String.Format("Error: A book already exists with the name {0}.", projectName), "projectName");
				return;
			}
			if (Directory.Exists(convertedProjectDirectory) && overwrite)
				Program.DeleteAllFilesAndFoldersInDirectory(convertedProjectDirectory);
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

			// Perhaps a bad assumption, but for now we will assume that if a non-English language
			// has the same number of elements as the English, that it is up-to-date. If not, don't include it.
			var englishTextElementCount = languageDictionary[Language.English].Count;
			var languagesToExclude = new List<Language>();
			foreach (var language in languageDictionary)
			{
				if (language.Value.Count != englishTextElementCount)
					languagesToExclude.Add(language.Key);
			}
			languageDictionary.RemoveAll(l => languagesToExclude.Contains(l.Key));

			for (int index = 0; index < languageDictionary[Language.English].Count; index++)
			{
				var list = new List<KeyValuePair<Language, string>>(6);
				foreach (var language in languageDictionary)
					list.Add(new KeyValuePair<Language, string>(language.Key, language.Value[index]));
				allLanguages.Add(list);
			}

			//Three things needed for a bloom book:
			//  book assets (images, narration audio, background audio)
			//  bloom book css and images
			//  the actual book, a generated html file built from the photostory project
			CopyAssetsAndResources(Path.GetDirectoryName(_projectXmlPath), convertedProjectDirectory);
			ConvertToBloom(photoStoryProject, Path.Combine(convertedProjectDirectory, String.Format("{0}.htm", projectName)), projectName, allLanguages);

			var hydrationArguments =
				$"hydrate --preset shellbook --bookpath \"{convertedProjectDirectory}\" --vernacularisocode en";
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
			Console.WriteLine("Successfully converted {0}", projectName);
		}

		//The assumption is that the wp3 archive only contains assets and a project.xml file. We convert the .xml file and copy the images and audio tracks.
		private void CopyAssetsAndResources(string sourceFolderPath, string destinationFolderPath)
		{
			var audioFiles = new List<Tuple<string, bool>>();
			foreach (var filePath in Directory.EnumerateFiles(sourceFolderPath))
			{
				var filename = Path.GetFileName(filePath);
				if (filename.Equals("project.xml"))
					continue;

				if (AudioHelper.AudioFileNeedsConversion(filename))
					audioFiles.Add(new Tuple<string, bool>(filename, true));
				else if (AudioHelper.IsAudioFile(filename))
					audioFiles.Add(new Tuple<string, bool>(filename, false));
				else
					// should be image assets
					File.Copy(Path.Combine(sourceFolderPath, filename), Path.Combine(destinationFolderPath, filename));
			}
			if (audioFiles.Count > 0)
				_audioHelper.ProcessAudioFiles(sourceFolderPath, destinationFolderPath, audioFiles);
		}

		//Pulls in all the gathered information for the project and creates a single bloom book html file at destinationFile
		private void ConvertToBloom(PhotoStoryProject project, string destinationFile, string bookName, IList<List<KeyValuePair<Language, string>>> text)
		{
			var document = new BloomDocument(project, bookName, Path.GetDirectoryName(destinationFile), text, _audioHelper.Duplicates);
			Ps3AndBloomSerializer.SerializeBloomHtml(document.ConvertToHtml(), destinationFile);
		}
	}
}
