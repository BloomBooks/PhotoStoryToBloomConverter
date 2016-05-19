using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Paragraph
    {
        [XmlText]
        public string Text;
    }
}