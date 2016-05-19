using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Span
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlAttribute("class")]
        public string Class;
        [XmlText]
        public string ContentText;
    }
}