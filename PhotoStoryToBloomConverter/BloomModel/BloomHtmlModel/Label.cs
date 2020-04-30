using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
	public class Label
	{
		[XmlAttribute("class")]
		public string Class;

		[XmlAttribute("lang")]
		public string LanguageCode;

		[XmlText]
		public string LabelText;

		public static Label CreateHintBubble(string text)
		{
			return new Label {Class = "bubble", LanguageCode = "en", LabelText = text};
		}
	}
}
