using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using PhotoStoryToBloomConverter.PS3Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to the 'Html' object
    public class BloomDocument
    {
        private readonly BloomMetadata _metadata;
        private readonly BloomBookData _bookData;
        private readonly List<BloomPage> _pages = new List<BloomPage>();
        private readonly IList<string> _text;

        public BloomDocument(PhotoStoryProject project, string bookName, string bookDirectoryPath, IList<string> narratedText)
        {
            _metadata = BloomMetadata.DefaultBloomMetadata(bookName);
            _bookData = BloomBookData.DefaultBloomBookData(bookName);

            //For each visual unit create a bloom page
            //Instead of creating a page for cover and credits pages, put information into the book data (data-div)
            //The X-Matter pack will create more visually appealing cover pages
            for (var i = 0; i < project.VisualUnits.Length; i++)
            {
                var visualUnit = project.VisualUnits[i];
                var psImage = visualUnit.Image;
                if (CreditsAndCoverExtractor.imageIsCreditsOrCover(psImage.Path))
                {
                    //If it was a credits page, put credit information into the data divs
                    if (CreditsAndCoverExtractor.extractedCreditString.Length > 0)
                    {
                        _bookData.LocalizedOriginalAcknowledgments.Add(CreditsAndCoverExtractor.extractedCreditString);
                        //Reset the extracted info, so we don't see it again
                        CreditsAndCoverExtractor.extractedCreditString = null;
                    }

                    //If the image had narration and/or background audio, and was the front cover, we want to store the audio for the new cover page
                    if (i == 0 && psImage.MusicTracks.Length > 0 && psImage.MusicTracks.First().SoundTracks.Length > 0)
                    {
                        _bookData.CoverBackgroundAudioPath = psImage.MusicTracks.First().SoundTracks.First().Path;
                        _bookData.CoverBackgroundAudioVolume = psImage.MusicTracks.First().Volume / 100.00;

                    }
                    if (i == 0 && visualUnit.Narration != null)
                        _bookData.CoverNarrationPath = visualUnit.Narration.Path;
                }
                else
                {
                    var cropRectangle = new Rectangle();
                    //If the image photostory was using had a crop edit, we need to adjust the image displayed for bloom likewise
                    if (psImage.Edits != null)
                    {
                        foreach (var edit in psImage.Edits)
                        {
                            if (edit.RotateAndCrop == null) continue;
                            cropRectangle = edit.RotateAndCrop.CroppedRect.ToAnimationRectangle();
                        }
                    }

                    var bloomImage = new BloomImage
                    {
                        Src = psImage.Path,
                        ImageSize = new Size { Height = psImage.AbsoluteMotion.BaseImageHeight, Width = psImage.AbsoluteMotion.BaseImageWidth },
                        ImageMotion = new BloomImageMotion
                        {
                            CropRectangle = cropRectangle,
                            InitialImageRectangle = psImage.AbsoluteMotion.Rects[0].ToAnimationRectangle(),
                            FinalImageRectangle = psImage.AbsoluteMotion.Rects[1].ToAnimationRectangle(),
                        }
                    };

                    var text = "";
                    if (narratedText.Count > i)
                        text = narratedText[i];

                    var backgroundPath = "";
                    var backgroundVolume = 0.00;
                    if (psImage.MusicTracks.Length > 0 && psImage.MusicTracks.First().SoundTracks.Length > 0)
                    {
                        backgroundPath = psImage.MusicTracks.First().SoundTracks.First().Path;
                        //Converting from 1-100 to 0.01-1.00
                        backgroundVolume = psImage.MusicTracks.First().Volume / 100.00;
                    }

                    var narrationPath = "";
                    if (visualUnit.Narration != null)
                        narrationPath = visualUnit.Narration.Path;

                    var bloomAudio = new BloomAudio(narrationPath, backgroundPath, backgroundVolume);

                    _pages.Add(new BloomPage(bloomImage, text, bloomAudio));
                }
            }
        }

        public Html ConvertToHtml()
        {
            var html = new Html
            {
                Head = _metadata.ConvertToHtml(),
                Body = new Body
                {
                    BookCreationType = "original",
                    Class = "publishMode",
                    Divs = new List<Div>
                    {
                        _bookData.ConvertToHtml(),
                    }
                }
            };
            foreach (var page in _pages)
                html.Body.Divs.Add(page.ConvertToHtml());
            return html;
        }
    }
}