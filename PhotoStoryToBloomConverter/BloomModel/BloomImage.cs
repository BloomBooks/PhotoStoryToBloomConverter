using System.Collections.Generic;
using System.Drawing;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to a div with the bloom tag 'bloom-imageContainer'
    public class BloomImage
    {
        public Size ImageSize;
        public string Src;
        public BloomImageMotion ImageMotion;

        public Div ConvertToHtml()
        {
            return new Div
            {
                Title = string.Format("{0} 0 KB {1} x {2} 0 DPI (should be 300-600) Bit Depth: 32",Src, ImageSize.Width, ImageSize.Height),
                Class = "bloom-imageContainer bloom-leadingElement",
                Divs = new List<Div>
                {
                    new Div
                    {
                        Class = "bloom-imageView",

                        DataInitialRect = string.Format("{0} {1} {2} {3}", ImageMotion.InitialImageRectangle.Left, ImageMotion.InitialImageRectangle.Top, ImageMotion.InitialImageRectangle.Width, ImageMotion.InitialImageRectangle.Height),
                        DataFinalRect = string.Format("{0} {1} {2} {3}", ImageMotion.FinalImageRectangle.Left, ImageMotion.FinalImageRectangle.Top, ImageMotion.FinalImageRectangle.Width, ImageMotion.FinalImageRectangle.Height),

                        Imgs = new[]
                        {
                            new Img
                            {
                                //Standard style for a single image on A4 Landscape
                                //The style interferes with the animations we are imposing...
                                //Style = "width: 689px; height: 677px; margin-left: 140px; margin-top: 0px;",
                                Src = Src,
                                Alt = string.Format("This picture, {0}, is missing or was loading too slowly.", Src),
                                Width = ImageSize.Width,
                                Height = ImageSize.Height,
                            }
                        }
                    }
                },
            };
        }
    }
}