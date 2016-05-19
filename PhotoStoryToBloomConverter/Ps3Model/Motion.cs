using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class Motion
    {
        [XmlAttribute("manual")]
        public int Manual;
        [XmlAttribute("workingImageWidth")]
        public int WorkingImageWidth;
        [XmlAttribute("workingImageHeight")]
        public int WorkingImageHeight;

        [XmlElement("Rect")]
        public Rect[] Rects;

        [XmlElement("RelativeRect")]
        public Rect[] RelativeRects;
    }
}