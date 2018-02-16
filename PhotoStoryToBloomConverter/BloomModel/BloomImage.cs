using System.Drawing;
using PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel;

namespace PhotoStoryToBloomConverter.BloomModel
{
	//Comparable to a div with the bloom tag 'bloom-imageContainer'
	public class BloomImage
	{
		public const bool kUseBackgroundImage = false;

		public Size ImageSize { get; set; }
		public string Src { get; set; }
		public BloomImageMotion ImageMotion { get; set; }

		public Div ConvertToHtml()
		{
			string classStr = null;
			string style = null;
			Img[] imgs = null;
			if (kUseBackgroundImage)
			{
				classStr = "bloom-imageContainer bloom-backgroundImage bloom-leadingElement";
				style = $"background-image:url('{Src}')";
			}
			else
			{
				classStr = "bloom-imageContainer bloom-leadingElement";
				imgs = new[]
				{
					new Img
					{
						Src = Src,
						Alt = $"This picture, {Src}, is missing or was loading too slowly.",
					}
				};
			}
			return new Div
			{
				Class = classStr,

				Style = style,

				Imgs = imgs,

				DataInitialRect = GetRectangleAttribute(ImageSize, ImageMotion.InitialImageRectangle),
				DataFinalRect = GetRectangleAttribute(ImageSize, ImageMotion.FinalImageRectangle)
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
