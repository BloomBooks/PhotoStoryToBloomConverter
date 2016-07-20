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

	    public string BloomTags { get; set; }
	    public string Uuid { get; set; }
	    public string DataPageLineage { get; set; }
	    public string Language { get; set; }
	    public string Text { get; set; }

	    public string PageLabel { get; set; }
	    public string PageDescription { get; set; }

	    public BloomPage(BloomImage image, IList<KeyValuePair<Language, string>> text, BloomAudio audio)
        {
			if (text == null)
				text = new List<KeyValuePair<Language, string>>();
            Uuid = Guid.NewGuid().ToString();
            BloomTags = "bloom-page numberedPage customPage Device16x9Portrait layout-style-Default bloom-monolingual";
            PageLabel = "";
            PageDescription = "";
            Language = "";
            ImageAndTextWithAudioSplitter = new BloomPageSplitter { Image = image, Text = text, Audio = audio };
        }

        public Div ConvertToHtml()
        {
            return new Div
            {
                Class = BloomTags,
                Id = Uuid,
                DataPageLineage = "",
                BackgroundAudio = GetBackgroundAudio(),
                BackgroundAudioVolume = string.IsNullOrWhiteSpace(ImageAndTextWithAudioSplitter.Audio.BackgroundAudioPath) ? null : ImageAndTextWithAudioSplitter.Audio.BackgroundVolume.ToString(CultureInfo.InvariantCulture),
                Lang = "",
                Divs = new List<Div>
                {
                    new Div {Class = "pageLabel", DataI18n = "", Lang = Language, SimpleText = PageLabel,},
                    new Div {Class = "pageDescription", Lang = Language, SimpleText = PageDescription},
                    new Div
                    {
                        Class = "marginBox",
                        Divs = new List<Div>
                        {
                            ImageAndTextWithAudioSplitter.ConvertToHtml()
                        }
                    }
                }
            };
        }

        public string GetBackgroundAudio()
        {
            if (!string.IsNullOrWhiteSpace(ImageAndTextWithAudioSplitter.Audio.BackgroundAudioPath))
                return ImageAndTextWithAudioSplitter.Audio.BackgroundAudioPath;
            return null;
        }
    }
}