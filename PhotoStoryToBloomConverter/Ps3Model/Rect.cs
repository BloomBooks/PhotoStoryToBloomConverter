using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class Rect
    {
        [XmlAttribute("upperLeftX")]
        public int UpperLeftX;
        [XmlAttribute("upperLeftY")]
        public int UpperLeftY;
        [XmlAttribute("width")]
        public float Width;
        [XmlAttribute("height")]
        public float Height;
        [XmlAttribute("weight")]
        public int Weight;
        [XmlAttribute("left")]
        public float Left;
        [XmlAttribute("right")]
        public float Right;
        [XmlAttribute("top")]
        public float Top;
    }
}