using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Span
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlAttribute("class")]
        public string Class;
		[XmlAttribute("data-duration")]
	    public string Duration;

        [XmlText]
        public string ContentText;
    }
}