using System;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using PhotoStoryToBloomConverter.PS3Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using NAudio.Wave;
using SIL.Windows.Forms.ClearShare;
using SIL.Windows.Forms.ImageToolbox;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to the 'Html' object
    public class BloomDocument
    {
        private readonly BloomMetadata _metadata;
        private readonly BloomBookData _bookData;
        private readonly List<BloomPage> _pages = new List<BloomPage>();
	    private readonly CreditsAndCoverExtractor.ImageIP _imageCopyrightAndLicense;

		public BloomDocument(PhotoStoryProject project, string bookName, string bookDirectoryPath, IList<List<KeyValuePair<Language, string>>> narratedText)
        {
            _metadata = BloomMetadata.DefaultBloomMetadata(bookName);
            _bookData = BloomBookData.DefaultBloomBookData(bookName);

            //A little bit of book-keeping, we want to remove images that are cover or credit images from the final directory
            var imagePathsToRemove = new SortedSet<string>();

	        bool creditsFound = false;

            //For each visual unit create a bloom page
            //Instead of creating a page for cover and credits pages, put information into the book data (data-div)
            //The X-Matter pack will create more visually appealing cover pages
            for (var i = 0; i < project.VisualUnits.Length; i++)
            {
				// With the latest templates (Jan 2017), we have been told that the next to last slide is always unnarrated
				// and only placed as a pause for reflection. We can ignore it.
	            if (i == project.VisualUnits.Length - 2)
		            continue;

                var visualUnit = project.VisualUnits[i];
                var psImage = visualUnit.Image;
                var backgroundAudioPath = GetBackgroundAudioPathForImage(psImage);
                var backgroundAudioVolume = (backgroundAudioPath == null)?0.00:GetBackgroundAudioVolumeForImage(psImage);

                var extractor = new CreditsAndCoverExtractor();
				extractor.Extract(bookName, Path.Combine(bookDirectoryPath, psImage.Path));
                if (extractor.IsCreditsOrCoverPage)
                {
                    //If it was a credits page, put credit information into the data divs
                    if (extractor.CreditString != null)
                    {
	                    creditsFound = true;
                        _bookData.LocalizedOriginalAcknowledgments.Add(extractor.CreditString);
	                    _imageCopyrightAndLicense = extractor.ImageCopyrightAndLicense;
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

                    List<KeyValuePair<Language, string>> text = null;
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
			if (!creditsFound)
				Console.WriteLine("Credits not processed for {0}", bookName);
			else
				CreditsAndCoverExtractor.CreateMapFile();

	        if (_imageCopyrightAndLicense != CreditsAndCoverExtractor.ImageIP.Unknown)
	        {
		        //Because credits may have been at end of book, go back through and set image credits if we extracted some.
		        foreach (var page in _pages)
		        {
			        var imageLocation = Path.Combine(bookDirectoryPath, page.ImageAndTextWithAudioSplitter.Image.Src);
			        using (var image = PalasoImage.FromFile(imageLocation))
			        {
				        if (_imageCopyrightAndLicense == CreditsAndCoverExtractor.ImageIP.SweetPublishingAndWycliffe && IsWycliffeImage(bookName, image.FileName))
					        ApplyWycliffeIPInfoForImages(image);
				        else
					        ApplySweetPublishingIPInfoForImages(image);

				        image.SaveUpdatedMetadataIfItMakesSense();
			        }
		        }
	        }
	        else
	        {
		        Debug.Fail("Image copyright and license information unknown for {0}", bookName);
	        }
            foreach (var imagePath in imagePathsToRemove)
            {
                File.Delete(imagePath);
            }
		}

		private bool IsWycliffeImage(string bookName, string imagePath)
		{
			// Unfortunate we can't get this information from the project in some consistent way.
			// For some of the images, the "comment" field includes the original file name but not all.
			// (If we knew all the original file names, we would know which ones are Wycliffe because they end in 'CD')
			var file = Path.GetFileNameWithoutExtension(imagePath);
			return (bookName.ToLowerInvariant() == "in the beginning" &&
				new HashSet<string> { "2", "3", "5", "7", "9", "11", "13", "14", "15", "16", "18", "19", "20", "21", "23", "25", "38", "39" }.Contains(file)) ||
					(bookName.ToLowerInvariant() == "sin enters the world" && new HashSet<string> { "5" }.Contains(file));
		}

		private void ApplySweetPublishingIPInfoForImages(PalasoImage image)
		{
			image.Metadata.CopyrightNotice = "© Sweet Publishing";
			image.Metadata.License = CreativeCommonsLicense.FromLicenseUrl("https://creativecommons.org/licenses/by-sa/4.0/");
			image.Metadata.Creator = "Jim Padgett (may have been skin-darkened or otherwise adapted by SIL - VM Productions)";
		}

		private void ApplyWycliffeIPInfoForImages(PalasoImage image)
		{
			image.Metadata.CopyrightNotice = "© Wycliffe Bible Translators, Inc.";
			image.Metadata.License = CreativeCommonsLicense.FromLicenseUrl("https://creativecommons.org/licenses/by-nc-nd/4.0/");
			image.Metadata.License.RightsStatement = "You may crop and resize but not modify the images for your new work. " +
				"Images may be rotated or flipped horizontally, provided this does not contradict historical fact or violate cultural norms.";
			image.Metadata.Creator = "Carolyn Dyk";
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