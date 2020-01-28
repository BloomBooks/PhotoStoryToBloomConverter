using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SIL.IO;

namespace PhotoStoryToBloomConverter.Utilities
{
	/// <summary>
	/// Copied and modified from BloomDesktop's BloomZipFile
	/// </summary>
	public static class ZipHelper
	{
		public static void Zip(string directoryPath, string outputPath)
		{

			var fsOut = RobustFile.Create(outputPath);
			ZipOutputStream zipStream = new ZipOutputStream(fsOut);

			AddDirectory(directoryPath, zipStream);

			zipStream.IsStreamOwner = true; // makes the Close() also close the underlying stream
			zipStream.Close();
		}


		/// <summary>
		/// Adds a directory, along with all files and subdirectories
		/// </summary>
		private static void AddDirectory(string directoryPath, ZipOutputStream zipStream)
		{
			var rootName = Path.GetFileName(directoryPath);
			if (rootName == null)
				return;

			var dirNameOffset = directoryPath.Length - rootName.Length;
			AddDirectory(directoryPath, dirNameOffset, zipStream);
		}

		private static void AddDirectory(string directoryPath, int dirNameOffset, ZipOutputStream zipStream)
		{
			var files = Directory.GetFiles(directoryPath);
			foreach (var path in files)
			{
				var entryName = path.Substring(dirNameOffset);
				entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
				AddFile(path, entryName, zipStream);
			}

			var folders = Directory.GetDirectories(directoryPath);

			foreach (var folder in folders)
			{
				var dirName = Path.GetFileName(folder);
				if (dirName == null)
					continue; // Don't want to bundle these up

				AddDirectory(folder, dirNameOffset, zipStream);
			}
		}

		private static void AddFile(string path, string entryName, ZipOutputStream zipStream)
		{
			var fi = new FileInfo(path);
			var newEntry = new ZipEntry(entryName) { DateTime = fi.LastWriteTime, Size = fi.Length, IsUnicodeText = true };

			zipStream.PutNextEntry(newEntry);

			// Zip the file in buffered chunks
			var buffer = new byte[4096];
			using (var streamReader = RobustFile.OpenRead(path))
			{
				StreamUtils.Copy(streamReader, zipStream, buffer);
			}

			zipStream.CloseEntry();
		}
	}
}
