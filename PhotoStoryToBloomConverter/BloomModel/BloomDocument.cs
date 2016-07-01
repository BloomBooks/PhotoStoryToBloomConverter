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
            if (CreditsExtractor.extractedCreditString.Length > 0) 
                _bookData.LocalizedOriginalAcknowledgments.Add(CreditsExtractor.extractedCreditString);
            _text = narratedText;

            var deletedPages = 0;
            for (var i = 0; i < project.VisualUnits.Length; i++)
            {
                var unit = project.VisualUnits[i];
                var image = unit.Image;

                var cropRectangle = new Rectangle();
                if (image.Edits != null)
                {
                    foreach (var edit in image.Edits)
                    {
                        if (edit.RotateAndCrop == null) continue;
                        cropRectangle = edit.RotateAndCrop.CroppedRect.ToAnimationRectangle();
                    }
                }

                var bloomImage = new BloomImage
                {
                    Src = image.Path,
                    ImageSize = new Size { Height = image.AbsoluteMotion.BaseImageHeight, Width = image.AbsoluteMotion.BaseImageWidth },
                    ImageMotion = new BloomImageMotion
                    {
                        CropRectangle = cropRectangle,
                        InitialImageRectangle = image.AbsoluteMotion.Rects[0].ToAnimationRectangle(),
                        FinalImageRectangle = image.AbsoluteMotion.Rects[1].ToAnimationRectangle(),
                    }
                };

                var backgroundPath = "";
                if (image.MusicTracks != null)
                {
                    foreach (var musicTrack in image.MusicTracks)
                    {
                        backgroundPath = musicTrack.SoundTracks.First().Path;
                    }
                }
                var narrationPath = "";
                if (unit.Narration != null)
                    narrationPath = unit.Narration.Path;

                var bloomAudio = new BloomAudio(narrationPath, backgroundPath);
                //If the image doesn't exist, we assume we removed it because it was a credit or cover image
                if (File.Exists(Path.Combine(bookDirectoryPath, image.Path)))
                    _pages.Add(BloomPage.BloomImageOnlyPage(bloomImage, bloomAudio));
                else
                {
                    deletedPages++;
                    if(i - deletedPages < _text.Count)
                        _text.RemoveAt(i - deletedPages); //Don't leave orphaned text, it will throw off the matching of text and image.
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
			IList<Div> divs = new List<Div>(_pages.Count);
	        for (int i = 0; i < _pages.Count; i++)
	        {
		        string pageText = null;
		        if (_text != null && _text.Count > i)
			        pageText = _text[i];
		        divs.Add(_pages[i].ConvertToHtml(pageText));
	        }
	        html.Body.Divs.AddRange(divs);
            return html;

        }
    }
}