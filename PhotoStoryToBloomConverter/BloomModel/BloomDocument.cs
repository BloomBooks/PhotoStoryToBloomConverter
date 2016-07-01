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
            _text = narratedText;

            var originalTextListLength = _text.Count;
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
                if (!imageIsCreditsOrCover(Path.Combine(bookDirectoryPath, image.Path)))
                    _pages.Add(BloomPage.BloomImageOnlyPage(bloomImage, bloomAudio));
                else if (i < originalTextListLength)
                    _text.RemoveAt(i); //Don't leave orphaned text, it will throw off the matching of text and image.
            }
        }

        private readonly string[] creditImageHashes = { "369F414F6F2CC91AE0AD0FF1CDCFAD21", "CDF13EC119AD0128E1196DB518B64BF8", "E3BF24BB5F62DFF39E7C2301E4ED2642", "56C6BDF749C9FE421C9BC6F832B7CCE0" };
        private readonly string[] coverImageHashes = { "8C7B5AADFF9AB8B4649481421EB8479F", "781ED3E63E6BD138D9BE59A24EFF7D6A" };

        private bool imageIsCreditsOrCover(string imagePath)
        {
            var md5Hash = BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(imagePath))).Replace("-", "");
            //Currently all of the information from the credit page needs to be stored in the additional acknowledgments section
            if (string.Equals(md5Hash, creditImageHashes[0]))
            {
                /* Stories adapted by VM Productions from
                 * READ-N-GROW PICTURE BIBLE
                 * and used by permission.
                 * Illustrations by Jim Padgett, Courtesy of
                 * Sweet Publishing, Ft. Worth, TX. ©2011
                 * and Carolyn Dyk.
                 * Skin-darkened by Lori MacLean and VM Productions. */
                _bookData.LocalizedOriginalAcknowledgments.Add("Stories adapted by VM Productions from READ-N-GROW PICTURE BIBLE and used by permission." + " " +
                                                               "Illustrations by Jim Padgett, Courtesy of Sweet Publishing, Ft. Worth, TX. ©2011 and Carolyn Dyk." + " " +
                                                               "Skin-darkened by Lori MacLean and VM Productions.");
                return true;
            }
            else if (string.Equals(md5Hash, creditImageHashes[1]))
            {
                /* Stories adapted by VM Productions from
                 * READ-N-GROW PICTURE BIBLE
                 * and used by permission.
                 * Illustrations by Jim Padgett, Courtesy of
                 * Sweet Publishing, Ft. Worth, TX. ©2011
                 * and Carolyn Dyk.
                 * Skin-darkened by Lori MacLean and VM Productions.
                 * Some images adapted by Beth Rupprecht. */
                _bookData.LocalizedOriginalAcknowledgments.Add("Stories adapted by VM Productions from READ-N-GROW PICTURE BIBLE and used by permission." + " " +
                                                               "Illustrations by Jim Padgett, Courtesy of Sweet Publishing, Ft. Worth, TX. ©2011 and Carolyn Dyk." + " " +
                                                               "Skin-darkened by Lori MacLean and VM Productions." + " " +
                                                               "Some images adapted by Beth Rupprecht.");
                return true;
            }
            //SinEnters and Ruth have the same credits, they are just different resolution images
            else if (string.Equals(md5Hash, creditImageHashes[2]) || string.Equals(md5Hash, creditImageHashes[3]))
            {
                /* Biblical Illustrations by Jim Padgett
                 * courtesy of Sweet Publishing, Ft. Worth, TX
                 * and Gospel Light, Ventura, CA
                 * Copyright 1984. All rights reserved.
                 * Additional illustrations by Carolyn Dyk. */
                _bookData.LocalizedOriginalAcknowledgments.Add("Biblical Illustrations by Jim Padgett courtesy of Sweet Publishing, Ft. Worth, TX and Gospel Light, Ventura, CA Copyright 1984. All rights reserved." + " " + 
                                                               "Additional illustrations by Carolyn Dyk.");
                return true;
            }

            foreach (var coverImageHash in coverImageHashes)
            {
                if (string.Equals(md5Hash, coverImageHash))
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