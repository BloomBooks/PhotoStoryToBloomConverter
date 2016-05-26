using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    //Usually only seen on the title slide
    public class TextOverlay
    {
        [XmlAttribute("text")]
        public string Text;
        [XmlAttribute("verticalAlignment")]
        public int VerticalAlignment;
        [XmlAttribute("horizontalAlignment")]
        public int HorizontalAlignment;
        [XmlAttribute("fontReferenceWidth")]
        public int FontReferenceWidth;
        [XmlAttribute("fontReferenceHeight")]
        public int FontReferenceHeight;

        [XmlElement("Font")]
        public Font[] Fonts;
    }
}