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
        public string Text;
        public float BackgroundVolume;

        public string PageLabel;
        public string PageDescription;

        public BloomPage(BloomImage image, string text, BloomAudio audio)
        {
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
                BackgroundAudioVolume = BackgroundVolume.ToString(),
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