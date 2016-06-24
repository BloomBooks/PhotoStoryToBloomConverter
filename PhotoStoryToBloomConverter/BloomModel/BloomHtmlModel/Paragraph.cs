using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Paragraph
    {
        [XmlElement("span")]
        public Span Span;

        [XmlText]
        public string Text;
    }
}