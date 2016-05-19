using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Meta
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("charset")]
        public string Charset;
        [XmlAttribute("content")]
        public string Content;
    }
}