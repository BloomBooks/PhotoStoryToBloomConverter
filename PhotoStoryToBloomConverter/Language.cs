using System;

namespace PhotoStoryToBloomConverter
{
	public enum Language
	{
		English,
		French,
		Korean,
		Portuguese,
		Spanish,
		Tagalog,
		Unknown
	}

	static class LanguageExtensions
	{
		public static string GetCode(this Language language)
		{
			switch (language)
			{
				case Language.English: return "en";
				case Language.French: return "fr";
				case Language.Korean: return "ko";
				case Language.Portuguese: return "pt";
				case Language.Spanish: return "es";
				case Language.Tagalog: return "tl";
				default:
					throw new ArgumentOutOfRangeException("language", language, null);
			}
		}

		public static Language GetLanguageFromFileNameWithoutExtension(this string fileNameWithoutExtension)
		{
			if (fileNameWithoutExtension.Length < 3)
				return Language.Unknown;

			var token = fileNameWithoutExtension.Substring(fileNameWithoutExtension.Length - 3);
			switch (token)
			{
				case "fra": return Language.French;
				case "kor": return Language.Korean;
				case "por": return Language.Portuguese;
				case "spa": return Language.Spanish;
				case "tgl": return Language.Tagalog;

				case "eng":
				default: return Language.English;
			}
		}
	}
}
