using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Label
    {
        [XmlAttribute("class")]
        public string Class;
        [XmlText]
        public string LabelText;
    }
}