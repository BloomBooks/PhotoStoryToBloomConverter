using System.Drawing;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
    //Comparable to a div with the bloom tag 'bloom-imageContainer'
    public class BloomImage
    {
	    public Size ImageSize { get; set; }
	    public string Src { get; set; }
	    public BloomImageMotion ImageMotion { get; set; }

	    public Div ConvertToHtml()
        {
            return new Div
            {
                Title = string.Format("{0} 0 KB {1} x {2} 0 DPI (should be 300-600) Bit Depth: 32", Src, ImageSize.Width, ImageSize.Height),
                Class = "bloom-imageContainer bloom-backgroundImage bloom-leadingElement",

				Style = string.Format("background-image:url('{0}')", Src),

                DataInitialRect = string.Format("{0} {1} {2} {3}", ImageMotion.InitialImageRectangle.Left, ImageMotion.InitialImageRectangle.Top, ImageMotion.InitialImageRectangle.Width, ImageMotion.InitialImageRectangle.Height),
                DataFinalRect = string.Format("{0} {1} {2} {3}", ImageMotion.FinalImageRectangle.Left, ImageMotion.FinalImageRectangle.Top, ImageMotion.FinalImageRectangle.Width, ImageMotion.FinalImageRectangle.Height),
            };
        }
    }
}