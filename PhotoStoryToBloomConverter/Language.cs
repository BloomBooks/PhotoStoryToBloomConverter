using System;

namespace PhotoStoryToBloomConverter
{
	public enum Language
	{
		English,
		Bislama,
		French,
		Korean,
		Portuguese,
		Spanish,
		Swahili,
		Tagalog,
		TokPisin,
		Vietnamese,
		Unknown
	}

	static class LanguageExtensions
	{
		public static string GetCode(this Language language)
		{
			switch (language)
			{
				case Language.English: return "en";
				case Language.Bislama: return "bi";
				case Language.French: return "fr";
				case Language.Korean: return "ko";
				case Language.Portuguese: return "pt";
				case Language.Spanish: return "es";
				case Language.Swahili: return "sw";
				case Language.Tagalog: return "tl";
				case Language.TokPisin: return "tpi";
				case Language.Vietnamese: return "vi";
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
			{
				case "bis": return Language.Bislama;
				case "fra": return Language.French;
				case "kor": return Language.Korean;
				case "por": return Language.Portuguese;
				case "spa": return Language.Spanish;
				case "swa": return Language.Swahili;
				case "tgl": return Language.Tagalog;
				case "tpi": return Language.TokPisin;
				case "vie": return Language.Vietnamese;

				default: return Language.English;
			}
		}
	}
}
