using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
	//Comparable to a collection of the bloom 'data' divs
	public class BloomBookData
	{
		public string LanguagesOfBook;
		public string Topic { get; set; }
		public string StyleNumberSequence;
		public string LicenseUrl = ""; //"http://creativecommons.org/licenses/by/4.0/";
		public string LicenseImage;
		public string LicenseNotes;
		public string Title;
		public string Copyright;
		public List<string> ContentLanguages;

		public string CoverImage { get; set; }
		public string CoverNarrationPath;
		public string CoverBackgroundAudioPath;
		public double CoverBackgroundAudioVolume;

		//All localized variables are expected to have similar indexes
		//to their languages location in ContentLanguages
		public List<string> LocalizedBookTitle;
		public string[] LocalizedSmallCoverCredits;
		public string[] LocalizedOriginalContributions;
		public string[] LocalizedFunding;
		public List<string> LocalizedOriginalAcknowledgments;
		public string[] LocalizedInsideFrontCover;
		public string[] LocalizedInsideBackCover;
		public string[] LocalizedOutsideBackCover;
		public string[] LocalizedLicenseDescription;

		public SpAppMetadata SpAppMetadata { get; set; }

		public List<List<Dictionary<string, string>>> LocalizedNarrationList;

		public static BloomBookData DefaultBloomBookData(string title)
		{
			return new BloomBookData
			{
				Title = title,
				LanguagesOfBook = "English",
				Topic = "Spiritual",
				StyleNumberSequence = "0",
				ContentLanguages = new List<string> { "en" },
				Copyright = $"© {DateTime.Now.Year} Wycliffe Bible Translators, Inc.",
				LicenseUrl = "http://creativecommons.org/licenses/by-sa/4.0",
				LicenseNotes = null,
				LocalizedBookTitle = new List<string> { "" },
				LocalizedSmallCoverCredits = new[] { "" },
				LocalizedOriginalContributions = new[] { "" },
				LocalizedFunding = new[] { "" },
				LocalizedOriginalAcknowledgments = new List<string>(),
				LocalizedInsideFrontCover = new[] { "" },
				LocalizedInsideBackCover = new[] { "" },
				LocalizedOutsideBackCover = new[] { "" },
				LocalizedLicenseDescription = new[] { "" },
			};
		}

		public Div ConvertToHtml()
		{
			var dataDiv = new Div
			{
				Id = "bloomDataDiv",
				Divs = new List<Div>
				{
					new Div { DataBook = "coverImage", Lang = "*", SimpleText = CoverImage},
					new Div { DataBook = "styleNumberSequence", Lang = "*", SimpleText = StyleNumberSequence},
					new Div { DataBook = "languagesOfBook", Lang = "*", SimpleText = LanguagesOfBook},
					new Div { DataBook = "copyright", Lang = "*", SimpleText = Copyright},
					//new Div { DataBook = "licenseNotes", Lang = "*", SimpleText = LicenseNotes },
					new Div { DataBook = "licenseUrl", Lang = "*", SimpleText = LicenseUrl },
					new Div { DataBook = "topic", Lang = "en", SimpleText = Topic},
				}
			};
			if (CoverBackgroundAudioPath != null)
				dataDiv.Divs.Add(new Div { DataXmatterPage = "frontCover", BackgroundAudio = GetBackgroundAudio(), BackgroundAudioVolume = CoverBackgroundAudioVolume.ToString(CultureInfo.InvariantCulture) });

			const string language1Code = "en";
			dataDiv.Divs.Add(new Div { DataBook = "contentLanguage1", Lang = "*", SimpleText = language1Code });

			dataDiv.Divs.AddRange(LocalizedBookTitle.Select((title, index) => new Div { DataBook = "bookTitle", Lang = ContentLanguages[index], FormattedText = new List<Paragraph> {Paragraph.GetParagraphForTextWithAudio(title, CoverNarrationPath) } }).ToArray());
			dataDiv.Divs.AddRange(LocalizedSmallCoverCredits.Select((credits, index) => new Div { DataBook = "smallCoverCredits", Lang = ContentLanguages[index], FormattedText = new List<Paragraph> { new Paragraph { Text = credits } } }).ToArray());
			dataDiv.Divs.AddRange(LocalizedOriginalContributions.Select((contributions, index) => new Div { DataBook = "originalContributions", Lang = ContentLanguages[index], FormattedText = new List<Paragraph> { new Paragraph { Text = contributions } } }).ToArray());
			dataDiv.Divs.AddRange(LocalizedOriginalAcknowledgments.Select((acknowledgments, index) => new Div { DataBook = "originalAcknowledgments", Lang = ContentLanguages[index], FormattedText = Paragraph.GetMultiParagraphFromString(acknowledgments) }).ToArray());
			dataDiv.Divs.AddRange(LocalizedFunding.Select((funding, index) => new Div { DataBook = "funding", Lang = ContentLanguages[index], FormattedText = new List<Paragraph> { new Paragraph { Text = funding } } }).ToArray());
			dataDiv.Divs.AddRange(LocalizedInsideFrontCover.Select((insideFrontCover, index) => new Div { DataBook = "insideFrontCover", Lang = ContentLanguages[index], FormattedText = new List<Paragraph> { new Paragraph { Text = insideFrontCover } } }).ToArray());
			dataDiv.Divs.AddRange(LocalizedInsideBackCover.Select((insideBackCover, index) => new Div { DataBook = "insideBackCover", Lang = ContentLanguages[index], FormattedText = new List<Paragraph> { new Paragraph { Text = insideBackCover } } }).ToArray());
			dataDiv.Divs.AddRange(LocalizedOutsideBackCover.Select((outsideBackCover, index) => new Div { DataBook = "outsideBackCover", Lang = ContentLanguages[index], FormattedText = new List<Paragraph> { new Paragraph { Text = outsideBackCover } } }).ToArray());

			if (Program.SpAppOutput)
			{
				dataDiv.Divs.AddRange(SpAppMetadata.GetSpAppMetadataForDataDiv(language1Code));
			}

			return dataDiv;
		}

		public string GetBackgroundAudio()
		{
			if (!string.IsNullOrWhiteSpace(CoverBackgroundAudioPath))
				return CoverBackgroundAudioPath;
			return null;
		}
	}
}
