using System.Drawing;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to a div with the bloom tag 'bloom-imageContainer'
    public class BloomImage
    {
        public Size ImageSize;
        public string Src;

        public BloomImage(string src, Size imageSize)
        {
            Src = src;
            ImageSize = imageSize;
        }

        public Div ConvertToHtml()
        {
            return new Div
            {
                Title = string.Format("{0} 0 KB 0 x 0 0 DPI (should be 300-600) Bit Depth: 32",Src),
                Class = "bloom-imageContainer bloom-leadingElement",
                Imgs = new[]
                {
                    new Img
                    {
                        Style = string.Format("width: {0}px; height: {1}px; margin-left: 34px; margin-top:0px;", ImageSize.Width, ImageSize.Height),
                        Src = Src,
                        Alt = string.Format("This picture, {0}, is missing or was loading too slowly.", Src)
                    }
                }
            };
        }
    }
}