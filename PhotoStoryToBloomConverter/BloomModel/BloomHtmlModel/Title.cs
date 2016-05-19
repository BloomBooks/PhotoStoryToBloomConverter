using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Title
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("content")]
        public string Content;
        [XmlText]
        public string TitleText;
    }
}