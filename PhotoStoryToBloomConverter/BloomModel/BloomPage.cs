using System;
using System.Collections.Generic;
using System.IO;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to a div with the bloom tag 'bloom-page'
    public class BloomPage
    {
        public BloomPageSplitter ImageAndTextWithAudioSplitter;

        public string BloomTags;
        public string Uuid;
        public string DataPageLineage;
        public string Language;

        public string PageLabel;
        public string PageDescription;

        public BloomPage()
        {
            Uuid = Guid.NewGuid().ToString();
        }

        public static BloomPage BloomImageOnlyPage(BloomImage image, BloomAudio audio)
        {
            return new BloomPage
            {
                BloomTags = "bloom-page numberedPage customPage A4Landscape layout-style-Default bloom-monolingual",
                PageLabel = "",
                PageDescription = "",
                Language = "",
                ImageAndTextWithAudioSplitter = new BloomPageSplitter { Image = image, Audio = audio }
            };
        }

        public Div ConvertToHtml()
        {
            return new Div
            {
                Class = BloomTags,
                Id = Uuid,
                DataPageLineage = "056B6F11-4A6C-4942-B2BC-8861E62B03B3",
                BackgroundAudio = GetBackgroundAudio(),
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
                return Path.Combine(BloomAudio.kAudioDirectory, ImageAndTextWithAudioSplitter.Audio.BackgroundAudioPath);
            return null;
        }
    }
}