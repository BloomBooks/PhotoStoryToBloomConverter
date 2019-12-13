using System;
using System.Collections.Generic;
using System.Globalization;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
	//Comparable to a div with the bloom tag 'bloom-page'
	public class BloomPage
	{
		public BloomPageSplitter ImageAndTextWithAudioSplitter { get; set; }

		public string PageClasses { get; set; }
		public string Uuid { get; set; }
		public string DataPageLineage { get; set; }
		public string Text { get; set; }

		public string PageLabel { get; set; }
		public string PageDescription { get; set; }

		public BloomPage(BloomImage image, IList<KeyValuePair<Language, SourceText>> allTranslationsOfThisPage, BloomAudio audio)
		{
			if (allTranslationsOfThisPage == null)
				allTranslationsOfThisPage = new List<KeyValuePair<Language, SourceText>>();
			Uuid = Guid.NewGuid().ToString();
			PageClasses = "bloom-page numberedPage customPage Device16x9Portrait layout-style-Default bloom-monolingual";
			PageLabel = "Basic Text & Picture";
			PageDescription = "";
			DataPageLineage = "adcd48df-e9ab-4a07-afd4-6a24d0398382";
			ImageAndTextWithAudioSplitter = new BloomPageSplitter { Image = image, Text = allTranslationsOfThisPage, Audio = audio };
		}

		public Div ConvertToHtml()
		{
			return new Div
			{
				Class = PageClasses,
				Id = Uuid,
				DataPageLineage = DataPageLineage,
				BackgroundAudio = GetBackgroundAudio(),
				BackgroundAudioVolume = string.IsNullOrWhiteSpace(ImageAndTextWithAudioSplitter?.Audio?.BackgroundAudioPath) ? null : ImageAndTextWithAudioSplitter.Audio.BackgroundVolume.ToString(CultureInfo.InvariantCulture),
				Lang = "",
				Divs = new List<Div>
				{
					new Div {Class = "pageLabel", DataI18n = "", Lang = "en", SimpleText = PageLabel,},
					new Div {Class = "pageDescription", Lang = "en", SimpleText = PageDescription},
					new Div
					{
						Class = "marginBox",
						Divs = new List<Div> { GetMarginBoxChildDiv() }
					}
				}
			};
		}

		public virtual Div GetMarginBoxChildDiv()
		{
			return ImageAndTextWithAudioSplitter.ConvertToHtml();
		}

		public string GetBackgroundAudio()
		{
			if (!string.IsNullOrWhiteSpace(ImageAndTextWithAudioSplitter?.Audio?.BackgroundAudioPath))
				return ImageAndTextWithAudioSplitter.Audio.BackgroundAudioPath;
			return null;
		}
	}
}
