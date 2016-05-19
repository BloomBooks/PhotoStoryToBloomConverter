using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Head
    {
        [XmlElement("title")]
        public Title Title;
        [XmlElement("style")]
        public Style[] Styles;
        [XmlElement("script")]
        public Script Script;
        [XmlElement("meta")]
        public Meta[] Metas;
        [XmlElement("link")]
        public Link[] Links;
    }
}