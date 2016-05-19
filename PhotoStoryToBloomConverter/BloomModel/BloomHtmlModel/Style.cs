using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Style
    {
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("title")]
        public string Title;
        [XmlText]
        public string Css;
    }
}