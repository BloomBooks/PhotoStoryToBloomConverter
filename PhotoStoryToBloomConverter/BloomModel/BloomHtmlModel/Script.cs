using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Script
    {
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("src")]
        public string Src;
    }
}