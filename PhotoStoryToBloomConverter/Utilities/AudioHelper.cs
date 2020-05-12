using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NAudio.Wave;
using PhotoStoryToBloomConverter.BloomModel;

namespace PhotoStoryToBloomConverter.Utilities
{
	internal class AudioHelper
	{
		private const bool kProcessAudio = true;
		private readonly Dictionary<string, string> _duplicates = new Dictionary<string, string>();

		public Dictionary<string, string> Duplicates => _duplicates;

		public void ProcessAudioFiles(string sourceFolderPath, string destinationFolderPath, List<Tuple<string, bool>> audioFiles)
		{
			if (!kProcessAudio)
				return;

			Directory.CreateDirectory(Path.Combine(destinationFolderPath, BloomAudio.kAudioDirectory));
			var hashToFilenameDictionary = new Dictionary<string, string>();
			foreach (var audioFile in audioFiles)
			{
				string filename = audioFile.Item1;
				bool needsConversion = audioFile.Item2;

				var fullSourcePath = Path.Combine(sourceFolderPath, filename);
				var md5Hash = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(fullSourcePath))).Replace("-", "");
				string baseFilename;
				if (hashToFilenameDictionary.TryGetValue(md5Hash, out baseFilename))
				{
					_duplicates.Add(filename, baseFilename);
					continue;
				}

				if (needsConversion)
				{
					var newFileName = ConvertAudioFile(fullSourcePath,
						Path.Combine(destinationFolderPath, BloomAudio.kAudioDirectory, Path.GetFileNameWithoutExtension(filename)));
					if (newFileName == null)
						throw new ApplicationException($"Unable to convert {fullSourcePath}.");
					hashToFilenameDictionary.Add(md5Hash, newFileName);
				}
				else
				{
					File.Copy(fullSourcePath, Path.Combine(destinationFolderPath, BloomAudio.kAudioDirectory, filename));
					hashToFilenameDictionary.Add(md5Hash, filename);
				}
			}
		}

		public static bool AudioFileNeedsConversion(string fileName)
		{
			return Path.GetExtension(fileName) == ".wav";
		}

		private static string ConvertAudioFile(string sourcePath, string targetPathWithoutExtension)
		{
			return new Mp3Encoder().Encode(sourcePath, targetPathWithoutExtension);
		}

		public static bool IsAudioFile(string fileName)
		{
			return new[] {".mp3", ".wav", ".wma"}.Contains(Path.GetExtension(fileName));
		}

		public static string GetDuration(string path)
		{
			if (File.Exists(path))
				return GetFormattedDuration(new AudioFileReader(path).TotalTime);
			var mp3Path = Path.ChangeExtension(path, "mp3");
			if (File.Exists(mp3Path))
				return GetFormattedDuration(new AudioFileReader(mp3Path).TotalTime);
			return "2"; // arbitrary, should not happen.
		}

		private static string GetFormattedDuration(TimeSpan duration)
		{
			return duration.TotalSeconds.ToString(CultureInfo.InvariantCulture);
		}
	}
}
