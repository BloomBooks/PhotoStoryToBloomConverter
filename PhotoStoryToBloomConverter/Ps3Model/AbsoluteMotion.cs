using System.Xml.Serialization;

namespace PhotoStoryToBloomConverter.PS3Model
{
    public class AbsoluteMotion
    {
        [XmlAttribute("manual")]
        public int Manual;
        [XmlAttribute("workingImageWidth")]
        public int BaseImageWidth;
        [XmlAttribute("workingImageHeight")]
        public int BaseImageHeight;

        [XmlElement("Rect")]
        public Rect[] Rects;
    }
}