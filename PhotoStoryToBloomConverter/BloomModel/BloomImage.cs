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

                DataInitialRect = GetRectangleAttribute(ImageSize, ImageMotion.InitialImageRectangle),
				DataFinalRect = GetRectangleAttribute(ImageSize, ImageMotion.FinalImageRectangle),
				DataDuration = "5000"
            };
        }

	    private string GetRectangleAttribute(Size size, Rectangle rect)
	    {
		    double width = size.Width;
		    double height = size.Height;
			return string.Format("{0:N4} {1:N4} {2:N4} {3:N4}",
				-rect.Left / width,
				-rect.Top / height,
				rect.Width / width,
				rect.Height / height);
	    }
    }
}