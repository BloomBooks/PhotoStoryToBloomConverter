using System.Xml;
using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Html
    {
        [XmlElement("head")]
        public Head Head;
        [XmlElement("body")]
        public Body Body;
    }
}