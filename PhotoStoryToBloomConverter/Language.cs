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
		TokPisin,
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
				case Language.TokPisin: return "tpi";
				default:
					throw new ArgumentOutOfRangeException(nameof(language), language, null);
			}
		}

		public static Language GetLanguageFromFileNameWithoutExtension(this string fileNameWithoutExtension)
		{
			fileNameWithoutExtension = fileNameWithoutExtension.Trim();

			if (fileNameWithoutExtension.Length < 3)
				return Language.Unknown;

			var token = fileNameWithoutExtension.Substring(fileNameWithoutExtension.Length - 3);
			switch (token.ToLowerInvariant())
			//switch (token.ToLowerInvariant())
			{
				case "fra": return Language.French;
				case "kor": return Language.Korean;
				case "por": return Language.Portuguese;
				case "spa": return Language.Spanish;
				case "tgl": return Language.Tagalog;
				case "tpi": return Language.TokPisin;

				default: return Language.English;
			}
		}
	}
}
