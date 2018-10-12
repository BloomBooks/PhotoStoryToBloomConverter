using System.IO;
using System.Linq;

namespace PhotoStoryToBloomConverter.Utilities
{
	public class IOHelper
	{
		public static string SanitizeFileOrDirectoryName(string fileOrDirectoryName)
		{
			fileOrDirectoryName = Path.GetInvalidFileNameChars().Aggregate(
				fileOrDirectoryName, (current, character) => current.Replace(character, '-'));
			return fileOrDirectoryName;
		}
	}
}
