using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;
using PhotoStoryToBloomConverter.PS3Model;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to the 'Html' object
    public class BloomDocument
    {
        private readonly BloomMetadata _metadata;
        private readonly BloomBookData _bookData;
        private readonly List<BloomPage> _pages = new List<BloomPage>();

        public BloomDocument(PhotoStoryProject project, string bookName)
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
                    ImageSize = new Size { Height = image.Height, Width = image.Width },
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
                _pages.Add(BloomPage.BloomImageOnlyPage(bloomImage, bloomAudio));
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
            html.Body.Divs.AddRange(_pages.Select(page => page.ConvertToHtml()));
            return html;

        }
    }
}