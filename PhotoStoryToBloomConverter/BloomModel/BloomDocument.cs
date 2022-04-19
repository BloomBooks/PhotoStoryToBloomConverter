using System;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using PhotoStoryToBloomConverter.PS3Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using PhotoStoryToBloomConverter.Utilities;

namespace PhotoStoryToBloomConverter.BloomModel
{
	//Comparable to the 'Html' object
	public class BloomDocument
	{
		private readonly BloomMetadata _metadata;
		private readonly BloomBookData _bookData;
		private readonly List<BloomPage> _pages = new List<BloomPage>();
		private readonly CreditsAndCoverExtractor.CreditsType _imageCreditsType;
		private readonly Dictionary<string, string> _duplicateAudioFiles;

		public BloomDocument(
			PhotoStoryProject project,
			string bookName,
			string bookDirectoryPath,
			IList<List<KeyValuePair<Language, SourceText>>> allPagesInAllLanguages,
			Dictionary<string, string> duplicateAudioFiles,
			SpAppMetadata spAppMetadata)
		{
			_metadata = BloomMetadata.DefaultBloomMetadata(bookName);
			_bookData = BloomBookData.DefaultBloomBookData(bookName);
			_duplicateAudioFiles = duplicateAudioFiles;

			//A little bit of book-keeping, we want to remove images that are cover or credit images from the final directory
			var imagePathsToRemove = new SortedSet<string>();

			bool creditsFound = false;
			bool coverImageFound = false;
			bool originalHasRealTitleGraphic = false; // as opposed to the normal gray image which we don't use

			//For each visual unit create a bloom page
			//Instead of creating a page for cover and credits pages, put information into the book data (data-div)
			//The X-Matter pack will create more visually appealing cover pages
			for (var iPage = 0; iPage < project.VisualUnits.Length; iPage++)
			{
				if (iPage >= allPagesInAllLanguages.Count)
				{
					Console.WriteLine($@"ERROR: Could not convert {bookName}. Number of slides from PhotoStory3 and Word document do not match.");
					continue;
					//throw new ApplicationException();
				}

				var isExplicitCreditsType = false;
				CreditsAndCoverExtractor.CreditsType explicitCreditsType = CreditsAndCoverExtractor.CreditsType.Unknown;

				// The last page may not have text, but we need to process the credits slide anyway.
				// Otherwise we can ignore pages without text (or with just an asterisk).
				var englishTextForPage = allPagesInAllLanguages[iPage].Single(kvp => kvp.Key == Language.English).Value;
				if (englishTextForPage.TextType == TextType.Text)
				{
					var englishTextForPageStr = englishTextForPage.Text;
					if (string.IsNullOrWhiteSpace(englishTextForPageStr) || englishTextForPageStr.Trim() == "*")
					{
						if (iPage != project.VisualUnits.Length - 1)
							continue;
					}
					else if (englishTextForPageStr.StartsWith("*CreditsType.") || englishTextForPageStr.StartsWith("CreditsType."))
					{
						var explicitCreditsTypeStr = englishTextForPageStr.Substring(englishTextForPageStr.IndexOf("CreditsType.") + "CreditsType.".Length);
						if (Enum.TryParse(explicitCreditsTypeStr, out explicitCreditsType)) { 
							isExplicitCreditsType = true;
							Console.WriteLine($"Credits type explicitly set in docx to \"{explicitCreditsTypeStr}\"");
						}
						else
						{
							Console.WriteLine($"ERROR: Credits type explicitly set in docx to \"{explicitCreditsTypeStr}\" which is not known. Will fall back to looking at the credits image.");
						}
					}
				}

				var visualUnit = project.VisualUnits[iPage];
				var psImage = visualUnit.Image;
				var backgroundAudioPath = GetBackgroundAudioPathForImage(psImage);
				var backgroundAudioVolume = backgroundAudioPath == null ? 0.0 : GetBackgroundAudioVolumeForImage(psImage);

				var extractor = new CreditsAndCoverExtractor();
				extractor.Extract(Path.Combine(bookDirectoryPath, psImage.Path));

				if (isExplicitCreditsType || extractor.IsCreditsOrCoverPage || iPage == 0)
				{
					//If it was a credits page, put credit information into the data divs
					if (isExplicitCreditsType || extractor.CreditString != null)
					{
						creditsFound = true;
						if (isExplicitCreditsType)
						{
							_bookData.LocalizedOriginalAcknowledgments.Add(CreditsAndCoverExtractor.GetCreditString(explicitCreditsType));
							_imageCreditsType = explicitCreditsType;
						}
						else
						{
							_bookData.LocalizedOriginalAcknowledgments.Add(extractor.CreditString);
							_imageCreditsType = extractor.ImageCopyrightAndLicense;
							Console.WriteLine($"Credits type determined from credits image: \"{_imageCreditsType}\"");
						}
					}

					//If the image had narration and/or background audio, and was the front cover, we want to store the audio for the new cover page
					if (iPage == 0 && backgroundAudioPath != null)
					{
						_bookData.CoverBackgroundAudioPath = backgroundAudioPath;
						_bookData.CoverBackgroundAudioVolume = backgroundAudioVolume;

					}
					if (iPage == 0 && visualUnit.Narration != null)
						_bookData.CoverNarrationPath = visualUnit.Narration.Path;

					if (iPage == 0)
					{
						Debug.Assert(allPagesInAllLanguages[iPage].All(kvp => kvp.Value.TextType == TextType.Title));
						SetContentLanguagesAndLocalizedTitles(allPagesInAllLanguages[iPage]);
					}

					if (extractor.IsCreditsOrCoverPage)
						imagePathsToRemove.Add(Path.Combine(bookDirectoryPath, psImage.Path));
					else if (iPage == 0)
					{
						_bookData.CoverImage = psImage.Path;
						coverImageFound = true;
						originalHasRealTitleGraphic = true;
					}
				}
				else
				{
					var cropRectangle = new Rectangle();
					//If the image PhotoStory was using had a crop edit, we need to adjust the image displayed for bloom likewise
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

					List<KeyValuePair<Language, SourceText>> allTranslationsOfThisPage = null;
					if (allPagesInAllLanguages.Count > iPage)
						allTranslationsOfThisPage = allPagesInAllLanguages[iPage].Select(kvp => new KeyValuePair<Language, SourceText>(kvp.Key, kvp.Value)).ToList();

					var narrationPath = "";
					if (visualUnit.Narration != null)
						narrationPath = visualUnit.Narration.Path;

					var narrationFilePath = Path.Combine(bookDirectoryPath, BloomAudio.kAudioDirectory, narrationPath);
					var bloomAudio = new BloomAudio(narrationPath, backgroundAudioPath, backgroundAudioVolume, AudioHelper.GetDuration(narrationFilePath));

					if (!coverImageFound)
					{
						_bookData.CoverImage = bloomImage.Src;
						coverImageFound = true;
					}

					_pages.Add(new BloomPage(bloomImage, allTranslationsOfThisPage, bloomAudio));
				}
			}
			if (!creditsFound)
				Console.WriteLine($@"ERROR: Credits not processed for {bookName}");
#if DEBUG
			// A helpful way to get the md5hashes of the images...
			// Can't do this on a user's machine due to lack of permission to write to the installed location.
			else
				CreditsAndCoverExtractor.CreateMapFile();
#endif

			if (_imageCreditsType != CreditsAndCoverExtractor.CreditsType.Unknown)
			{
				// Go back through and set image credits if we extracted some.
				var imageSources = _pages.Where(p => !(p is BloomTranslationInstructionsPage))
					.Select(p => p.ImageAndTextWithAudioSplitter.Image.Src).ToList();
				if (originalHasRealTitleGraphic)
					imageSources.Add(_bookData.CoverImage);
				foreach (var imageSource in imageSources)
				{
					var imageLocation = Path.Combine(bookDirectoryPath, imageSource);
					ImageUtilities.ApplyImageIpInfo(bookName, imageLocation, _imageCreditsType);
				}
			}
			else
			{
				Console.WriteLine($@"ERROR: Image copyright and license information unknown for {bookName}");
			}
			foreach (var imagePath in imagePathsToRemove)
			{
				File.Delete(imagePath);
			}

			// For now, only SPApp wants these extra pages.
			if (Program.SpAppOutput)
			{
				string coverNarrationPath = null;
				if (!string.IsNullOrWhiteSpace(_bookData.CoverNarrationPath))
					coverNarrationPath = Path.Combine(bookDirectoryPath, BloomAudio.kAudioDirectory, _bookData.CoverNarrationPath);
				AddTranslationInstructionPages();

				spAppMetadata.Graphic = originalHasRealTitleGraphic ? SpAppMetadataGraphic.FrontCoverGraphic : SpAppMetadataGraphic.GrayBackground;
				spAppMetadata.PrepareNarrationAudio(coverNarrationPath);
				_bookData.SpAppMetadata = spAppMetadata;
			}
		}

		private void AddTranslationInstructionPages()
		{
			_pages.InsertRange(0, BloomTranslationInstructionsPage.GetDefaultTranslationInstructionPages());
		}

		private void SetContentLanguagesAndLocalizedTitles(List<KeyValuePair<Language, SourceText>> allTitles)
		{
			var primaryLanguageTitleAndReference = allTitles.Single(kvp => kvp.Key.GetCode() == Program.PrimaryOutputLanguage).Value;
			var primaryLanguageTitleFromText = primaryLanguageTitleAndReference.Text;

			//PrimaryOutputLanguage needs to be first.
			//The order of the rest doesn't matter as long as it is the same between ContentLanguages and LocalizedBookTitle.
			_bookData.ContentLanguages[0] = Program.PrimaryOutputLanguage;
			_bookData.LocalizedBookTitle[0] = primaryLanguageTitleFromText;
			_bookData.LocalizedSmallCoverCredits[0] = primaryLanguageTitleAndReference.Reference;

			_bookData.Title = primaryLanguageTitleFromText;

			var notPrimary = allTitles.Where(kvp => kvp.Key.GetCode() != Program.PrimaryOutputLanguage).ToList();
			_bookData.ContentLanguages.AddRange(notPrimary.Select(page => page.Key.GetCode()));
			_bookData.LocalizedBookTitle.AddRange(notPrimary.Select(page => page.Value.Text));
		}

		private double GetBackgroundAudioVolumeForImage(Ps3Image image)
		{
			//Converting from 1-100 to 0.01-1.00
			return image.MusicTracks.First().Volume / 100.00;
		}

		private string GetBackgroundAudioPathForImage(Ps3Image image)
		{
			if (image.MusicTracks?.First().SoundTracks != null)
			{
				var audioPath = image.MusicTracks.First().SoundTracks.First().Path;
				if (_duplicateAudioFiles.TryGetValue(audioPath, out var realPath))
					return realPath;
				return audioPath;
			}
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
