using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using PhotoStoryToBloomConverter.PS3Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NAudio.Wave;
using SIL.Windows.Forms;
using SIL.Windows.Forms.ClearShare;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to the 'Html' object
    public class BloomDocument
    {
        private readonly BloomMetadata _metadata;
        private readonly BloomBookData _bookData;
        private readonly List<BloomPage> _pages = new List<BloomPage>();
        private string ImageCopyright;
        private string ImageLicense;
        private string ImageCreator;

        public BloomDocument(PhotoStoryProject project, string bookName, string bookDirectoryPath, IList<string> narratedText)
        {
            _metadata = BloomMetadata.DefaultBloomMetadata(bookName);
            _bookData = BloomBookData.DefaultBloomBookData(bookName);

            //A little bit of book-keeping, we want to remove images that are cover or credit images from the final directory
            var imagePathsToRemove = new SortedSet<string>();

            //For each visual unit create a bloom page
            //Instead of creating a page for cover and credits pages, put information into the book data (data-div)
            //The X-Matter pack will create more visually appealing cover pages
            for (var i = 0; i < project.VisualUnits.Length; i++)
            {
                var visualUnit = project.VisualUnits[i];
                var psImage = visualUnit.Image;
                var backgroundAudioPath = GetBackgroundAudioPathForImage(psImage);
                var backgroundAudioVolume = (backgroundAudioPath == null)?0.00:GetBackgroundAudioVolumeForImage(psImage);
                if (CreditsAndCoverExtractor.imageIsCreditsOrCover(Path.Combine(bookDirectoryPath, psImage.Path)))
                {
                    //If it was a credits page, put credit information into the data divs
                    if (CreditsAndCoverExtractor.extractedCreditString != null)
                    {
                        _bookData.LocalizedOriginalAcknowledgments.Add(CreditsAndCoverExtractor.extractedCreditString);

                        if (CreditsAndCoverExtractor.extractedImageCopyright != null)
                            ImageCopyright = CreditsAndCoverExtractor.extractedImageCopyright;
                        if (CreditsAndCoverExtractor.extractedImageLicense != null)
                            ImageLicense = CreditsAndCoverExtractor.extractedImageLicense;
                        if (CreditsAndCoverExtractor.extractedImageCreator != null)
                            ImageCreator = CreditsAndCoverExtractor.extractedImageCreator;

                        //Reset the extracted info, so we don't see it again
                        CreditsAndCoverExtractor.extractedCreditString = null;
                        CreditsAndCoverExtractor.extractedImageCopyright = null;
                        CreditsAndCoverExtractor.extractedImageLicense = null;
                        CreditsAndCoverExtractor.extractedImageCreator = null;

                    }

                    //If the image had narration and/or background audio, and was the front cover, we want to store the audio for the new cover page
                    if (i == 0 && backgroundAudioPath != null)
                    {
                        _bookData.CoverBackgroundAudioPath = backgroundAudioPath;
                        _bookData.CoverBackgroundAudioVolume = backgroundAudioVolume;

                    }
                    if (i == 0 && visualUnit.Narration != null)
                        _bookData.CoverNarrationPath = visualUnit.Narration.Path;

                    imagePathsToRemove.Add(Path.Combine(bookDirectoryPath, psImage.Path));
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
                    if (narratedText != null && narratedText.Count > i)
                        text = narratedText[i];

                    var narrationPath = "";
                    if (visualUnit.Narration != null)
                        narrationPath = visualUnit.Narration.Path;

	                var narrationFilePath = Path.Combine(bookDirectoryPath, BloomAudio.kAudioDirectory, narrationPath);
                    var bloomAudio = new BloomAudio(narrationPath, backgroundAudioPath, backgroundAudioVolume, GetDuration(narrationFilePath));

                    _pages.Add(new BloomPage(bloomImage, text, bloomAudio));
                }
            }
            //Because credits may have been at end of book, go back through and set image credits if we extracted some.
            foreach (var page in _pages)
            {
                var imageLocation = Path.Combine(bookDirectoryPath, page.ImageAndTextWithAudioSplitter.Image.Src);
                using (var image = SIL.Windows.Forms.ImageToolbox.PalasoImage.FromFile(imageLocation))
                {
                    image.Metadata.CopyrightNotice = ImageCopyright;
                    image.Metadata.License = new CreativeCommonsLicense(true, true, CreativeCommonsLicense.DerivativeRules.DerivativesWithShareAndShareAlike);
                    image.Metadata.Creator = ImageCreator;
                    image.SaveUpdatedMetadataIfItMakesSense();
                }
            }
            foreach (var imagePath in imagePathsToRemove)
            {
                File.Delete(imagePath);
            }
        }

		private string GetDuration(string path)
		{
			if (File.Exists(path))
				return new AudioFileReader(path).TotalTime.ToString();
			var mp3Path = Path.ChangeExtension(path, "mp3");
			if (File.Exists(mp3Path))
				return new AudioFileReader(mp3Path).TotalTime.TotalSeconds.ToString();
			return "2"; // arbitrary, should not happen.
		}

		private double GetBackgroundAudioVolumeForImage(Ps3Image image)
        {
            //Converting from 1-100 to 0.01-1.00
            return image.MusicTracks.First().Volume / 100.00;
        }

        private string GetBackgroundAudioPathForImage(Ps3Image image)
        {
            if (image.MusicTracks != null && image.MusicTracks.First().SoundTracks != null)
                return image.MusicTracks.First().SoundTracks.First().Path;
            return null;
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