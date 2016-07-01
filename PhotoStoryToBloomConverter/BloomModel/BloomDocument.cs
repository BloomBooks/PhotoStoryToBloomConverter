using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using PhotoStoryToBloomConverter.PS3Model;
using System.IO;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to the 'Html' object
    public class BloomDocument
    {
        private readonly BloomMetadata _metadata;
        private readonly BloomBookData _bookData;
        private readonly List<BloomPage> _pages = new List<BloomPage>();

        public BloomDocument(PhotoStoryProject project, string bookName, string bookDirectoryPath)
        {
            _metadata = BloomMetadata.DefaultBloomMetadata(bookName);
            _bookData = BloomBookData.DefaultBloomBookData(bookName);

            foreach (var unit in project.VisualUnits)
            {
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
                if (!imageIsCreditsOrCover(Path.Combine(bookDirectoryPath, image.Path)))
                    _pages.Add(BloomPage.BloomImageOnlyPage(bloomImage, bloomAudio));
            }
        }

        private readonly string[] creditImages = { "Credit-Beginning.jpg", "Credit-SinEnters.jpg", "Credit-Moses.jpg", "Credit-Ruth.jpg" };
        private readonly string[] coverImages = { "Cover-Beginning.jpg", "Cover-Lazarus.png" };

        private bool imageIsCreditsOrCover(string imagePath)
        {
            foreach (var creditImagePath in creditImages)
            {
                if (FileCompare(imagePath, Path.Combine("Credit Images", creditImagePath)))
                    return true;
            }
            foreach (var coverImagePath in coverImages)
            {
                if (FileCompare(imagePath, Path.Combine("Cover Images", coverImagePath)))
                    return true;
            }
            return false;
        }

        private bool FileCompare(string file1, string file2)
        {

            if (file1 == file2) return true;

            FileStream fs1 = new FileStream(file1, FileMode.Open);
            FileStream fs2 = new FileStream(file2, FileMode.Open);

            if (fs1.Length != fs2.Length)
            {
                fs1.Close();
                fs2.Close();
                return false;
            }

            int file1byte;
            int file2byte;

            do
            {
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            fs1.Close();
            fs2.Close();

            return ((file1byte - file2byte) == 0);
        }

        public Html ConvertToHtml(IList<string> text)
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
		        if (text != null && text.Count > i)
			        pageText = text[i];
		        divs.Add(_pages[i].ConvertToHtml(pageText));
	        }
	        html.Body.Divs.AddRange(divs);
            return html;

        }
    }
}