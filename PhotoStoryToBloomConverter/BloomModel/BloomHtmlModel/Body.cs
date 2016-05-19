using System.Collections.Generic;
using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.BloomModel.BloomHtmlModel
{
    public class Body
    {
        [XmlAttribute("bookcreationtype")]
        public string BookCreationType;
        [XmlAttribute("class")]
        public string Class;
        [XmlElement("div")]
        public List<Div> Divs;
    }
}