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
                Title = $"{Src} 0 KB 0 x 0 0 DPI (should be 300-600) Bit Depth: 32",
                Class = "bloom-imageContainer bloom-leadingElement",
                Imgs = new[]
                {
                    new Img
                    {
                        Style = $"width: {ImageSize.Width}px; height: {ImageSize.Height}px; margin-left: 34px; margin-top:0px;",
                        Src = Src,
                        Alt = $"This picture, {Src}, is missing or was loading too slowly."
                    }
                }
            };
        }
    }
}